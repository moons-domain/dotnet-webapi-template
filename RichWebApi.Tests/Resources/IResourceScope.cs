namespace RichWebApi.Tests.Resources;

public interface IResourceScope : IDisposable
{
	IResourceScope CreateScope(string scope);

	string Scope { get; }

	Stream GetResourceStream(string nameSubstring);
}