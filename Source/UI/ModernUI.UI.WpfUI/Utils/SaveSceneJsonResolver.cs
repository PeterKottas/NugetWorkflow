using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NugetWorkflow.UI.WpfUI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public class SaveSceneJsonResolver : DefaultContractResolver
    {

        public static readonly SaveSceneJsonResolver Instance = new SaveSceneJsonResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var attributes = member.GetCustomAttributes().Where(a => a.GetType() == typeof(SaveSceneAttribute));
            if (attributes.Count() > 0)
            {
                property.ShouldSerialize = instance => true;
            }
            else
            {
                property.ShouldSerialize = instance => false;
            }
            return property;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            if (objectType == typeof(SecureString))
            {
                contract.Converter = new JsonSecureStringConverter();
            }
            return contract;
        }
    }
}
