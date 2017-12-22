using System;

namespace Samarium.PluginFramework.Config {

    /// <summary >
    /// Empty interface defining an empty container object for
    /// configuration properties.
    /// Can be used with Json.Net and YamlDotNet to serialize configs.
    /// 
    /// This interface is already marked with the <see cref="ConfigContainerAttribute"/> attribute.
    /// </summary>
    /// <seealso cref="ConfigMemberAttribute"/>
    /// <example >
    /// <code >
    /// public class MyConfigContainer: IConfigContainer {
    /// 
    ///     [ConfigMember(Name = "my_int_config")]
    ///     public int MyIntConfig { get; set; }
    ///     
    ///     [ConfigMember(Name = "my_object_config")]
    ///     public object MyObjectConfig { get; set; }
    /// 
    /// }
    /// </code>
    /// </example>
    [ConfigContainer]
    public interface IConfigContainer { }
}
