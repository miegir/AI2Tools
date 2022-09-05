namespace AI2Tools;

public partial interface IResource
{
    IEnumerable<Action> BeginUnpack(UnpackArguments arguments);
    IEnumerable<Action> BeginUnroll();
}
