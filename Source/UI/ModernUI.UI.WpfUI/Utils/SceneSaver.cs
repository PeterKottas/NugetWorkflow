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

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class SceneSaver
    {
        private static HomePageViewModel homeVm
        {
            get
            {
                return ViewModelService.GetViewModel<HomePageViewModel>();
            }
        }
        private static string lastFileSavedPath = null;

        public static string LastFileSavedPath
        {
            get
            {
                return lastFileSavedPath;
            }
            set
            {
                lastFileSavedPath = value;
            }
        }

        public static void SaveUI()
        {
            var dlg = new CommonSaveFileDialog();
            dlg.Title = "Save your current scene";
            dlg.AddToMostRecentlyUsedList = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.ShowPlacesList = true;
            dlg.Filters.Add(new CommonFileDialogFilter("Scene", "scn"));
            dlg.DefaultExtension = "scn";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SceneSaver.Save(dlg.FileName);
            }
            dlg.Dispose();
            SceneSaver.MakeClean();
        }

        public static void OpenUI()
        {
            if (HandleDirtySceneOverWrite())
            {
                var dlg = new CommonOpenFileDialog();
                dlg.Title = "Choose scene to open";
                dlg.IsFolderPicker = false;
                dlg.Multiselect = false;
                dlg.Filters.Add(new CommonFileDialogFilter("Scene", "scn"));
                dlg.DefaultExtension = "scn";

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SceneSaver.Load(dlg.FileName);
                }
                dlg.Dispose();
                SceneSaver.MakeClean();
            }
        }

        public static void Save(string path = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    SaveUI();
                }
                else
                {
                    var viewDictionary = ViewModelService.ViewDictionary;
                    var result = JsonConvert.SerializeObject(viewDictionary,
                        new JsonSerializerSettings { ContractResolver = SaveSceneJsonResolver.Instance, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
                    File.WriteAllText(path, result);
                }
            }
            catch (Exception e)
            {

                throw new SceneSaveException("Exception while trying to save the scene", e);
            }

        }

        private static object LoadObject(JToken source, Type destinationType)
        {
            if (source.Type == JTokenType.Object)
            {
                var sourceObject = (JObject)source;
                var destinationProps = destinationType.GetProperties().Where(prop => prop.GetCustomAttributes(false).Where(a => a.GetType() == typeof(SaveSceneAttribute)).Count() > 0).ToList();
                var destination = Activator.CreateInstance(destinationType);
                foreach (var destinationProp in destinationProps)
                {
                    var value = sourceObject.GetValue(destinationProp.Name);
                    if (value != null)
                    {
                        destinationProp.SetValue(destination, LoadObject(value, destinationProp.PropertyType));
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
                if (destinationType == typeof(SecureString))
                {
                    return ((JProperty)source).Value.ToString().Unprotect().ToSecuredString();
                }
                else
                {
                    return ((JProperty)source).Value;
                }
            }
            else
            {
                if (destinationType == typeof(SecureString))
                {
                    return ((JValue)source).Value.ToString().Unprotect().ToSecuredString();
                }
                else
                {
                    return ((JValue)source).Value;
                }
            }
        }

        public static void Load(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                var jObject = JsonConvert.DeserializeObject<Dictionary<Type, JObject>>(json, new JsonSerializerSettings { ContractResolver = SaveSceneJsonResolver.Instance });
                var dict = new Dictionary<Type, object>();
                foreach (var item in jObject)
                {
                    dict.Add(item.Key, LoadObject(item.Value, item.Key));
                }
                ViewModelService.ViewDictionary = dict;
                PageUserControlService.ReassignViewModels();
                MakeClean();
            }
            catch (Exception e)
            {
                throw new SceneLoadException("Exception while trying to load the scene", e);
            }
        }

        public static void MakeDirty(object parrent, string PropertyName)
        {
            if (!homeVm.IsDirty && HomePageViewModel.IsDirtyPropName != PropertyName)
            {
                var prop = parrent.GetType().GetProperty(PropertyName);
                if (prop != null)
                {
                    var atr = prop.GetCustomAttributes(typeof(SaveSceneAttribute), false);
                    if (atr != null && atr.Count() > 0)
                    {
                        homeVm.IsDirty = true;
                    }
                }
            }
        }

        public static bool HandleDirtySceneOverWrite()
        {
            if (homeVm.IsDirty)
            {
                var res = ModernDialog.ShowMessage("Save current scene?", "Unsaved changes", System.Windows.MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    Save();
                    return true;
                }
                else if (res == MessageBoxResult.No)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static void MakeClean()
        {
            homeVm.IsDirty = false;
        }
    }
}
