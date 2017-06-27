using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Events;

public class LoadCSV : MonoBehaviour {

    public CSVFile[] filePaths;

    public List<FileData> fileData;

    public UnityEvent OnDataLoaded = new UnityEvent();

    [System.Serializable]
    public struct CSVFile
    {
        [Header("Required:")]
        public int dataID;
        public string path;
    }
		
    [System.Serializable]
    public class FileData
    {
        public string name;

        public LineData[] lineData;

        public int dataID;

        public FileData (string _name, string fileData)
        {
            name = _name;
            string[] newLineData = fileData.Split("\n"[0]);

            lineData = new LineData[newLineData.Length];

            int i = 0;
		
            while (i < lineData.Length)
            {
                lineData[i] = new LineData(newLineData[i]);
                i++;
            }
        }
       
    }

    [System.Serializable]
    public class LineData
    {
        public CellData[] cellData;

        public LineData (string lineData)
        {
            string[] newCellData = (lineData.Trim()).Split(","[0]);

            cellData = new CellData[newCellData.Length];

            int i = 0;

            while(i < cellData.Length)
            {
                cellData[i] = new CellData(newCellData[i]);
                i++;
            }
        }
    }

    [System.Serializable]
    public class CellData
    {
		public int data;

        public CellData(string newData)
        {
			int.TryParse(newData,out data);	
        }
    }


    void Start()
    {

        LoadAllFiles();
    }

    void LoadAllFiles()
    {
        for (int i = 0; i < filePaths.Length; i++)
        {
            LoadData(filePaths[i]);
        }

        if (OnDataLoaded != null)
        {
            OnDataLoaded.Invoke();
        }
    }

    void LoadData(CSVFile csvFile)
    {
        if (csvFile.path == "") return;
        string[] fileName = (csvFile.path.Trim()).Split("\\"[0]);

        Debug.Log("Loading file: " + csvFile.path + ".");

        FileData newFileData = new FileData(fileName[fileName.Length - 1], File.ReadAllText(Path.Combine(Application.streamingAssetsPath ,csvFile.path)));
        newFileData.dataID = csvFile.dataID;
        fileData.Add(newFileData);
    }

    public FileData GetData(int dataID)
    {
        for (int i = 0; i < fileData.Count; i++)
        {
            if (dataID == fileData[i].dataID)
            {
                return fileData[i];
            }
        }

        return null;
    }

	public Vector2 GetDimensions(int dataID)
	{
		FileData fileData = GetData (dataID);

		if (fileData == null) 
		{
			Debug.LogError ("Dimensions requested for invalid dataID " + dataID.ToString () + ". Returning Vector2.zero");
			return Vector2.zero;
		}

		if (fileData.lineData.Length == 0) 
		{
			Debug.LogWarning ("Dimensions requested for dataID " + dataID.ToString () + ". DataID exists but has no data");
			return Vector2.zero;
		}

		return new Vector2(fileData.lineData.Length, fileData.lineData[0].cellData.Length);
			
	}

}