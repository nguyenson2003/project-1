using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class dataSheet{
    public AudioClip music;
    public TextAsset left,right;
    public TextAsset infoSound;
    public string getNameSong(){
        return infoSound.text.Split('\n')[0].Trim();
    }
}

public class SongManager : MonoBehaviour
{
    public AudioSource baseAudioSource;
    [SerializeField] dataSheet sheet;
    ArrayList []sheetCat = new ArrayList[2];
    // ArrayList sheetWhiteCat = new ArrayList();
    int []curIndex = {-1, -1};
    string nameSong = "abc";
    float TickPerSecond;
    // Start is called before the first frame update
    private void Awake()
    {
       setSong(sheet);
    }
    public void setSong(dataSheet sheet){
        this.sheet=sheet;
        baseAudioSource.clip = sheet.music;
        
        sheetCat[0] = new ArrayList();
        sheetCat[0].AddRange(sheet.left.text.Split('\n'));

        sheetCat[1] = new ArrayList();
        sheetCat[1].AddRange(sheet.right.text.Split('\n'));
        
        string[] readAllLineInfo = sheet.infoSound.text.Split('\n');
        nameSong = readAllLineInfo[0].Trim();
        TickPerSecond = float.Parse(readAllLineInfo[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
    }
    public float loadNextSecond(int side){
        if(curIndex[side]>=sheetCat[side].Count-1)return -1;
        int tickBef = curIndex[side]<0?0:int.Parse(((string)sheetCat[side][curIndex[side]]).Trim().Split()[0]);
        int tickAft = int.Parse(((string)sheetCat[side][++curIndex[side]]).Trim().Split()[0]);
        return (tickAft-tickBef)/TickPerSecond;
    }
    public float loadCurEndSecond(int side){
        if(curIndex[side]>=sheetCat[side].Count)return -1;
        int tickBef = int.Parse(((string)sheetCat[side][curIndex[side]]).Trim().Split()[0]);
        int tickAft = int.Parse(((string)sheetCat[side][curIndex[side]]).Trim().Split()[1]);
        return (tickAft-tickBef)/TickPerSecond;
    }
    public void play(){
        if(!baseAudioSource.isPlaying){
            baseAudioSource.clip = sheet.music;
            baseAudioSource.Play();
        }
    }
    public void stop(){
        baseAudioSource.Stop();
        curIndex[0]=-1;
        curIndex[1]=-1;
    }
    public void pause(){
        baseAudioSource.Pause();
    }
    public float getPercent(){
        return Mathf.Max(curIndex[0]+curIndex[1]+2,0)*1.0f/(sheetCat[0].Count+sheetCat[1].Count);
    }
    public string getNameSong(){
        return nameSong;
    }
}
