namespace AI2Tools;

public partial interface IResource
{
    Action? BeginUnpack(UnpackArguments arguments);
    Action? BeginUnroll();
}
