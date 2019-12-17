using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleFileBrowser;

public class MainMenu : MonoBehaviour
{
	void Start()
    {
		//Set Cursor to not be visible
        Cursor.visible = true;
    }
	
    public void ExitApplicaion()
	{
		Application.Quit();
	}
	
	public void LoadMainScene()
	{
		// Set filters (optional)
		// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
		// if all the dialogs will be using the same filters
		FileBrowser.SetFilters(true, new FileBrowser.Filter("network", ".json"));

		// Set default filter that is selected when the dialog is shown (optional)
		// Returns true if the default filter is set successfully
		// In this case, set Images filter as the default filter
		FileBrowser.SetDefaultFilter( ".json" );

		// Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
		// Note that when you use this function, .lnk and .tmp extensions will no longer be
		// excluded unless you explicitly add them as parameters to the function
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe");
		
		// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
		// It is sufficient to add a quick link just once
		// Name: Work
		// Path: C:\Work
		// Icon: default (folder icon)
		FileBrowser.AddQuickLink("Work", "C:\\Work", null);
		
		// Coroutine example
		StartCoroutine(ShowLoadDialogCoroutine());
	}
	
	private IEnumerator ShowLoadDialogCoroutine()
	{
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load" );

		// Dialog is closed
		if(FileBrowser.Success)
		{
			// Print whether a file is chosen (FileBrowser.Success)
			// and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
			//Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
			
			if(NetworkManager.LoadNetwork(FileBrowser.Result)) SceneManager.LoadScene("MainScene");
		}
	}
}
