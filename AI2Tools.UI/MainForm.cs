using System.Reflection;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class MainForm : Form
{
    private record ManifestResource(Assembly Assembly, string Name)
    {
        public override string ToString() => Name;

        public Stream OpenRead() => Assembly.GetManifestResourceStream(Name)
            ?? throw new InvalidOperationException($"Stream not found: '{Name}'.");
    }

    private record ActionContext(
        ManifestResource Resource,
        GamePipeline Pipeline,
        CancellationToken CancellationToken = default);

    private static readonly Properties.Settings Settings = Properties.Settings.Default;

    private readonly ILogger logger;
    private Game? game;

    public MainForm()
    {
        InitializeComponent();

        logger = new ListBoxLogger(LogBox);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        PopulateResources();

        var gamePath = Settings.GamePath;
        if (!string.IsNullOrEmpty(gamePath))
        {
            OpenGame(gamePath);
        }
    }

    private void GamePathBrowseButton_Click(object sender, EventArgs e)
    {
        if (GamePathBrowseDialog.ShowDialog() == DialogResult.OK)
        {
            OpenGame(GamePathBrowseDialog.FileName);
        }
    }

    private void PopulateResources()
    {
        ResourceNameBox.Items.Clear();

        var assembly = GetType().Assembly;

        foreach (var name in assembly.GetManifestResourceNames())
        {
            if (!name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var index = ResourceNameBox.Items.Add(new ManifestResource(assembly, name));

            if (Settings.ResourceName == name)
            {
                ResourceNameBox.SelectedIndex = index;
            }
        }

        if (ResourceNameBox.Items.Count > 0 && ResourceNameBox.SelectedIndex < 0)
        {
            ResourceNameBox.SelectedIndex = 0;
        }

        UpdateActions();
    }

    private void OpenGame(string gamePath)
    {
        GamePathBox.Text = gamePath;

        game = new Game(logger, gamePath);

        if (UpdateActions())
        {
            RollButton.Focus();
        }

        Settings.GamePath = gamePath;
        Settings.Save();
    }

    private bool UpdateActions()
    {
        var canPatch = game != null && ResourceNameBox.SelectedIndex >= 0;

        RollButton.Enabled = canPatch;
        UnrollButton.Enabled = canPatch;
        CancellationButton.Enabled = false;

        UpdateVersionInfo();

        return canPatch;
    }

    private void ResourceNameBox_SelectionChangeCommitted(object sender, EventArgs e)
    {
        var resourceName = ResourceNameBox.Text;
        if (!string.IsNullOrEmpty(resourceName))
        {
            Settings.ResourceName = resourceName;
            Settings.Save();
        }
    }

    private void PerformAction(Action<ActionContext> action, bool canCancel = false)
    {
        if (game == null)
        {
            return;
        }

        if (ResourceNameBox.SelectedItem is not ManifestResource selectedResource)
        {
            return;
        }

        var cts = new CancellationTokenSource();

        var context = new ActionContext(
            Pipeline: game.CreatePipeline(),
            Resource: selectedResource,
            CancellationToken: cts.Token);

        Enable(false);
        LogBox.Items.Clear();
        CancellationButton.Click += CancelHandler;

        Task.Factory.StartNew(() =>
        {
            try
            {
                action(context);

                logger.LogInformation("Done.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().ToString());
            }
        }, cts.Token).ContinueWith(_ =>
        {
            BeginInvoke(() =>
            {
                CancellationButton.Click -= CancelHandler;
                Enable(true);
            });
            cts.Dispose();
        });

        void Enable(bool enable)
        {
            ResourceNameLabel.Enabled = enable;
            ResourceNameBox.Enabled = enable;
            GamePathLabel.Enabled = enable;
            GamePathBox.Enabled = enable;
            GamePathBrowseButton.Enabled = enable;
            RollButton.Enabled = enable;
            UnrollButton.Enabled = enable;
            CancellationButton.Enabled = !enable && canCancel;
        }

        void CancelHandler(object? sender, EventArgs e)
        {
            cts.Cancel();
        }
    }

    private void RollButton_Click(object sender, EventArgs e)
    {
        PerformAction(context =>
        {
            using var stream = context.Resource.OpenRead();
            using var container = new ObjectContainer(stream);
            context.Pipeline.Unpack(
                new UnpackArguments(
                    Container: container));
        });
    }

    private void UnrollButton_Click(object sender, EventArgs e)
    {
        PerformAction(context => context.Pipeline.Unroll());
    }

    private void UpdateVersionInfo()
    {
        if (game is not null)
        {
            var version = game.GetVersionInfo().GameVersion;
            GameVersionBox.Text = version.ProductVersionString;
        }
        else
        {
            GameVersionBox.Text = "<none>";
        }

        if (ResourceNameBox.SelectedItem is ManifestResource selectedResource)
        {
            using var stream = selectedResource.OpenRead();
            using var container = new ObjectContainer(stream);
            var version = container.FindGameVersion();
            ResourceVersionBox.Text = version?.ProductVersionString ?? "<not found>";
        }
        else
        {
            ResourceVersionBox.Text = "<none>";
        }
    }
}
