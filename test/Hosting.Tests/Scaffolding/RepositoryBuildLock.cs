namespace Hosting.Tests.Scaffolding;

/// <summary>
///     A lock, held across processes, on building projects of the repository: a file in <c>artifacts/hosting-tests</c> opened without
///     sharing. The test processes of both target frameworks see the same file, so only one of them builds at a time.
/// </summary>
internal sealed class RepositoryBuildLock : IAsyncDisposable
{
    private RepositoryBuildLock(FileStream file) => _file = file;

    /// <summary>
    ///     Waits until no other test, in this process or another, holds the lock, and takes it.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The lock, released when it is disposed.</returns>
    public static async Task<RepositoryBuildLock> Acquire(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LockFile)!);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return new(new FileStream(LockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None));
            }
            catch (IOException)
            {
                // another build has it
                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
            }
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => _file.DisposeAsync();

    private static readonly string LockFile = Path.Combine(Repository.Root, "artifacts", "hosting-tests", ".build.lock");

    private readonly FileStream _file;
}
