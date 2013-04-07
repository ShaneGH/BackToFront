
namespace BackToFront.Enum
{
    /// <summary>
    /// Describes how the ValidateMemberAttribute handles dependencies
    /// </summary>
    public enum DependencyBehavior
    {
        /// <summary>
        /// When validating a property, the rules which require dependencies will be ignored
        /// </summary>
        IgnoreRulesWithDependencies,
        /// <summary>
        /// When validating the property, rules which require dependencies will not be given them, forcing rules to searcdh for dependencies via dependency injection
        /// </summary>
        UseInbuiltDI,
        /// <summary>
        /// When validating the property, rules which require dependencies will be given them if they exist in the ServiceContainer. Otherwise DI is used
        /// </summary>
        UseServiceContainerAndInbuiltDI
    }
}
