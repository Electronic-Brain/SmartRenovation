/* This script may not be reproduced or selling anywhere without prior written permission of Electronic Brain.
 * Project Developed by - 
 * Srejon Khan 
 * Game Programmer, Electronic Brain 
 * 
 * Ashikur Rahman 
 * Game Programmer,Electronic Brain
 * Email : support@electronicbrain.net 
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SmartEditor : EditorWindow
{
    public List<string> filePaths = new List<string>();
    public List<string> folderPaths = new List<string>();
    public string destinationPath;

    [MenuItem("Electronic Brain/Smart Library/Smart Renovation")]
    public static void ShowWindow()
    {
        GetWindow<SmartEditor>(false, "Smart Renovation", true);
    }
    void OnGUI()
    {
        #region Header
        var headerStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        GUILayout.Label("Smart Renovation", headerStyle);
        GUILayout.Label("Smartly Renovate your Plugins", headerStyle);
        #endregion

        #region Source Folders
        GUILayout.Label("Source Folders", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        if (folderPaths != null)
        {
            for (int i = 0; i < folderPaths.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField("Source Folder No." + (i + 1), folderPaths[i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    folderPaths.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Add Source Folders"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            if (path != null)
            {
                folderPaths.Add(path);
            }
        }
        #endregion

        #region Source Files
        GUILayout.Label("Source Files", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        if (filePaths != null)
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField("Source File No." + (i + 1), filePaths[i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    filePaths.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Add Source File"))
        {
            string path = EditorUtility.OpenFilePanel("Select File", "", "");
            if (path != null)
            {
                filePaths.Add(path);
            }
        }
        #endregion

        #region Utilities
        GUILayout.Label("Utilities", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Source Paths"))
        {
            string pathsString = null;
            for (int i = 0; i < filePaths.Count; i++)
            {
                pathsString += @filePaths[i] + ",";
            }
            pathsString += "•";
            for (int i = 0; i < folderPaths.Count; i++)
            {
                pathsString += @folderPaths[i] + ",";
            }
            FilesPath filesPath = new FilesPath
            {
                paths = pathsString
            };
            string jsonData = JsonUtility.ToJson(filesPath);

            string savePath = EditorUtility.SaveFilePanel("Save Current Path Data", "", "PathsData", "json");
            System.IO.File.WriteAllText(savePath, jsonData);
            this.ShowNotification(new GUIContent("Successfully Saved!"));
        }
        if (GUILayout.Button("Load Source Paths"))
        {
            string loadPath = EditorUtility.OpenFilePanel("Select File", "", "json");
            string loadData = System.IO.File.ReadAllText(loadPath);

            FilesPath filesPath = JsonUtility.FromJson<FilesPath>(loadData);

            string[] parts = filesPath.paths.Split('•');
            string[] splittedFilesPath = parts[0].Split(',');
            string[] splittedFoldersPath = parts[1].Split(',');
            filePaths.Clear();

            for (int i = 0; i < splittedFilesPath.Length; i++)
            {
                if (i != splittedFilesPath.Length - 1) filePaths.Add(splittedFilesPath[i]);
            }
            for (int i = 0; i < splittedFoldersPath.Length; i++)
            {
                if (i != splittedFoldersPath.Length - 1) folderPaths.Add(splittedFoldersPath[i]);
            }
            this.ShowNotification(new GUIContent("Successfully Loaded!"));
        }
        EditorGUILayout.EndHorizontal();

        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.red;
        if (GUILayout.Button("Clear All", style))
        {
            if (EditorUtility.DisplayDialog("Clear", "Are you sure you want to clear all selected files and folder?", "Clear All", "Do Not Clear"))
            {
                filePaths.Clear();
                folderPaths.Clear();
                this.ShowNotification(new GUIContent("Successfully Cleared!"));
            }
        }
        #endregion

        #region Destination
        GUILayout.Label("Destination", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Destination Path", destinationPath);
        if (GUILayout.Button("Select Destination Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Destination Folder", "", "");
            if (path != null)
            {
                destinationPath = path;
            }
        }
        #endregion

        #region Actions
        GUILayout.Label("Actions", EditorStyles.boldLabel);
        if (GUILayout.Button("Update All"))
        {
            if (EditorUtility.DisplayDialog("Update All Files & Folders", "Are you sure you want to update all selected files and folder?", "Update All", "Do Not Update All"))
            {
                CopyFiles();
                CopyFolders();
                this.ShowNotification(new GUIContent("Successfully updated! Please refresh if needed."));
            }
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Files Only"))
        {
            if (EditorUtility.DisplayDialog("Update All Files", "Are you sure you want to update all selected files?", "Update All", "Do Not Update All"))
            {
                CopyFiles();
                this.ShowNotification(new GUIContent("Successfully updated! Please refresh if needed."));
            }
        }
        if (GUILayout.Button("Update Folders Only"))
        {
            if (EditorUtility.DisplayDialog("Update All Folders", "Are you sure you want to update all selected Folders?", "Update All", "Do Not Update All"))
            {
                CopyFolders();
                this.ShowNotification(new GUIContent("Successfully updated! Please refresh if needed."));
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    void CopyFiles()
    {
        for (int i = 0; i < filePaths.Count; i++)
        {
            string path = System.IO.Path.GetDirectoryName(filePaths[i]);
            System.IO.File.Copy(filePaths[i], destinationPath + "/" + System.IO.Path.GetFileName(filePaths[i]), true);
        }
        AssetDatabase.Refresh();
    }
    void CopyFolders()
    {
        for (int i = 0; i < folderPaths.Count; i++)
        {
            foreach (string dir in System.IO.Directory.GetDirectories(folderPaths[i], "*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.Directory.CreateDirectory(dir.Replace(folderPaths[i], destinationPath + dir));
            }
            foreach (string newDir in System.IO.Directory.GetFiles(folderPaths[i], "*.*", System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.Copy(newDir, newDir.Replace(folderPaths[i], destinationPath), true);
            }
        }
        AssetDatabase.Refresh();
    }
    private class FilesPath
    {
        public string paths;
    }
}
