using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NugetWorkflow.Common.Base.Extensions;
using System.Collections;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models;
using System.IO;
using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using Microsoft.WindowsAPICodePack.Dialogs;
using FirstFloor.ModernUI.Windows.Controls;
using NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.General;
using System.Windows.Media;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class ConfigSaver
    {
        private static readonly string defaultPath = "..\\config.scnc";
        private static readonly string defaultPathLastSaved = "..\\lastSaved.txt";

        private static HomePageViewModel homeVm
        {
            get
            {
                return ViewModelService.GetViewModel<HomePageViewModel>();
            }
        }

        private static GeneralSettingsViewModel generalSettingsVm
        {
            get
            {
                return ViewModelService.GetViewModel<GeneralSettingsViewModel>();
            }
        }

        public static void OpenUI()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Choose config to open";
            dlg.IsFolderPicker = false;
            dlg.Multiselect = false;
            dlg.Filters.Add(new CommonFileDialogFilter("Scene config", "scnc"));
            dlg.DefaultExtension = "scnc";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SceneSaver.Load(dlg.FileName);
            }
            dlg.Dispose();
            SceneSaver.MakeClean();
        }

        public static void Save()
        {
            try
            {
                var viewDictionary = ViewModelService.ViewDictionary.Where(a => a.Key.GetCustomAttributes(typeof(SaveConfigAttribute), false).Count() > 0).ToDictionary(p => p.Key, p => p.Value);
                var result = JsonConvert.SerializeObject(viewDictionary,
                    new JsonSerializerSettings { ContractResolver = SaveConfigJsonResolver.Instance, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
                File.WriteAllText(defaultPath, result);
                ConfigSaver.MakeClean();
            }
            catch (Exception e)
            {

                throw new SceneSaveException("Exception while trying to save the config", e);
            }
        }

        public static void SaveLastSavedScene(string path)
        {
            try
            {
                generalSettingsVm.LastSavedScene = path;
                File.WriteAllText(defaultPathLastSaved, path);
            }
            catch (Exception e)
            {

                throw new SceneSaveException("Exception while trying to save the config", e);
            }
        }

        public static void LoadLastSavedScene()
        {
            try
            {
                if (File.Exists(defaultPathLastSaved))
                {
                    var path = File.ReadAllText(defaultPathLastSaved);
                    generalSettingsVm.LastSavedScene = path;
                }
            }
            catch (Exception e)
            {

                throw new SceneSaveException("Exception while trying to save the config", e);
            }
        }

        private static object LoadObject(JToken source, Type destinationType)
        {
            if (source.Type == JTokenType.Object)
            {
                var sourceObject = (JObject)source;
                var destinationProps = destinationType.GetProperties().Where(prop => prop.GetCustomAttributes(false).Where(a => a.GetType() == typeof(SaveConfigAttribute)).Count() > 0).ToList();
                var destination = Activator.CreateInstance(destinationType);
                foreach (var destinationProp in destinationProps)
                {
                    var value = sourceObject.GetValue(destinationProp.Name);
                    if (value != null)
                    {
                        destinationProp.SetValue(destination, Convert.ChangeType(LoadObject(value, destinationProp.PropertyType), destinationProp.PropertyType));
                    }
                }
                return destination;
            }
            else if (source.Type == JTokenType.Array)
            {
                var sourceObject = (JArray)source;
                var destination = (IList)Activator.CreateInstance(destinationType);
                if (destination is IList)
                {
                    var itemType = destination.GetType().GetGenericArguments()[0];
                    foreach (var sourceObjectItem in sourceObject)
                    {
                        destination.Add(LoadObject(sourceObjectItem, itemType));
                    }
                }
                return destination;
            }
            else if (source.Type == JTokenType.Property)
            {
                return HandlePropOrVal(((JProperty)source).Value, destinationType);
            }
            else
            {
                return HandlePropOrVal(((JValue)source).Value, destinationType);
            }
        }

        private static object HandlePropOrVal(object source, Type destinationType)
        {
            if (destinationType == typeof(SecureString))
            {
                return source.ToString().Unprotect().ToSecuredString();
            }
            else if (destinationType == typeof(Color))
            {
                return ColorConverter.ConvertFromString(source.ToString());
            }
            else
            {
                return source;
            }
        }

        public static void Load(string path = null)
        {
            try
            {
                if (path == null)
                {
                    path = defaultPath;
                }
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var jObject = JsonConvert.DeserializeObject<Dictionary<Type, JObject>>(json, new JsonSerializerSettings { ContractResolver = SaveConfigJsonResolver.Instance });
                    var dict = new Dictionary<Type, object>();
                    foreach (var item in jObject)
                    {
                        dict.Add(item.Key, LoadObject(item.Value, item.Key));
                    }
                    var dictScenes = dict.Where(a => a.Key.GetCustomAttributes(typeof(SaveConfigAttribute), false).Count() > 0);
                    foreach (var dictScene in dictScenes)
                    {
                        ViewModelService.ViewDictionary.Remove(dictScene.Key);
                        ViewModelService.ViewDictionary.Add(dictScene.Key, dictScene.Value);
                    }
                    PageUserControlService.ReassignViewModels();
                    MakeClean();
                }
            }
            catch (Exception e)
            {
                throw new SceneLoadException("Exception while trying to load the config", e);
            }
        }

        public static void MakeDirty(object parent, string PropertyName)
        {
            SceneSaver.MakeDirty(parent, PropertyName);
        }

        public static void MakeClean()
        {
            homeVm.IsConfigDirty = false;
        }
    }
}
