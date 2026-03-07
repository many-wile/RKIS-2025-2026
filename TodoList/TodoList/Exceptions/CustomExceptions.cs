using System;
namespace TodoList.Exceptions
{
	public class InvalidCommandException : Exception { public InvalidCommandException(string message) : base(message) { } }
	public class InvalidArgumentException : Exception { public InvalidArgumentException(string message) : base(message) { } }
	public class TaskNotFoundException : Exception { public TaskNotFoundException(string message) : base(message) { } }
	public class ProfileNotFoundException : Exception { public ProfileNotFoundException(string message) : base(message) { } }
	public class AuthenticationException : Exception { public AuthenticationException(string message) : base(message) { } }
	public class DuplicateLoginException : Exception { public DuplicateLoginException(string message) : base(message) { } }
	public class DataStorageException : Exception
	{
		public DataStorageException(string message, Exception innerException = null) : base(message, innerException) { }
	}
	public class DataAccessException : DataStorageException
	{
		public DataAccessException(string message, Exception innerException = null) : base(message, innerException) { }
	}
	public class DataEncryptionException : DataStorageException
	{
		public DataEncryptionException(string message, Exception innerException = null) : base(message, innerException) { }
	}
	public class DataCorruptionException : DataStorageException
	{
		public DataCorruptionException(string message, Exception innerException = null) : base(message, innerException) { }
	}
}