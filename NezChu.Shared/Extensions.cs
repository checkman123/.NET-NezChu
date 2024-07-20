using System.Reflection;

namespace NezChu.Shared
{
    public static class Extensions
    {
        #region Get SourceRevisionId

        /// <summary>
        /// Gets the source revision ID from the assembly's informational version. MSBuild appends this
        /// to the end of the informational version by default if the MSBuild variable is set.
        /// </summary>
        /// <returns></returns>
        public static string? GetSourceRevisionId(this Assembly? assembly)
        {
            return assembly
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion.Split('+').Skip(1).FirstOrDefault()
                ?[..8];
        }
        #endregion
    }
}
