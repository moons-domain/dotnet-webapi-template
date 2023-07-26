namespace RichWebApi.Tests.Resources;

public interface IResourceScope : IDisposable
{
	IResourceScope BeginScope(string scope);

	Stream GetResourceStream(string nameSubstring);
}