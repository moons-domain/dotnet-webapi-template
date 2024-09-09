namespace RichWebApi.Exceptions;

public abstract class DatabaseDependencyException(string message) : DependencyException(message);