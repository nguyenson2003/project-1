using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text txt_msg;
    [SerializeField] private Text txt_score;
    [SerializeField] private Button btn_play;
    [SerializeField] private SongManager loadSound;
    [SerializeField] private GameObject[] Note;
    [SerializeField] private GameObject[] perfectLine;
    private Queue<GameObject>[] waitNote = new Queue<GameObject>[2];
    private GameObject[] lastInsertWait = new GameObject[2];
    private int speedNote = 4; //tốc độ note nhạc (1s đi đc 4 World X)
    private float delayTime = 5;// sau delayTime giây, nhạc bắt đầu chạy
    private bool pause = true;
    private int score = 0;
    public bool canTouch = false;
    private static GameManager ins;
    public static GameManager getIns() { return ins; }
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        ins = this;
        //khoi tao
        waitNote[0] = new Queue<GameObject>();
        waitNote[1] = new Queue<GameObject>();
        beforeStartGame();
        btn_play.GetComponent<Button>().onClick.AddListener(startGame);

    }
    void beforeStartGame()
    {
        //clear queue
        GameObject temp;
        while (waitNote[0].TryDequeue(out temp)) GameObject.Destroy(temp);
        while (waitNote[1].TryDequeue(out temp)) GameObject.Destroy(temp);
        lastInsertWait[0] = null;
        lastInsertWait[1] = null;
        loadSound.stop();
        createNewNote(0);
        createNewNote(1);

        setScore(0);
        canTouch = true;
    }
    void startGame()
    {
        pause = false;
        StartCoroutine(PlayDelay());
    }
    // Update is called once per frame
    void Update()
    {
        if (pause == false)
        {
            updatePosNote();
            if(Input.GetKeyDown(KeyCode.F)){
                ClickNote(0);
            }
            if(Input.GetKeyDown(KeyCode.J)){
                ClickNote(1);
            }
            for(int side=0;side<2;side++){
                //nếu chưa hết nhạc
                if (lastInsertWait[side] != null)
                {
                    //tạo note tiếp theo
                    if (Mathf.Abs(lastInsertWait[side].transform.position.x) < 10)
                        createNewNote(side);
                }
                //qua vach perfect
                if (waitNote[side].Count != 0)
                {
                    if (Mathf.Abs(waitNote[side].Peek().transform.position.x)+Note[side].GetComponent<CircleCollider2D>().radius< Mathf.Abs(perfectLine[side].transform.position.x)
                    
                    ){
                        Miss(side);
                    }
                }

            }

            if (waitNote[0].Count == 0 && waitNote[1].Count == 0)
            {
                Win();
                return;
            }
        }
    }
    IEnumerator PlayDelay(){
        yield return new  WaitForSeconds(delayTime);
        loadSound.play();
    }
    void createNewNote(int side)
    {
        GameObject nextNote;
        //Khi 2 bên không có note nhạc (ví dụ khi bắt đầu game)
        if (lastInsertWait[side] == null)
        {
            float nextX = delayTime+loadSound.loadNextSecond(side);
            if (nextX == -1) return;
            nextNote = Instantiate<GameObject>(Note[side]);
            nextNote.transform.position = new Vector3(
                (nextX * speedNote +0.8f)*-Mathf.Pow(-1,side)+perfectLine[side].transform.position.x,
                perfectLine[side].transform.position.y,
                0
            );
        }
        //Ngược lại: thêm ở note nhạc tiếp theo
        else
        {
            float nextX = loadSound.loadNextSecond(side);// note tiếp theo sẽ xuất hiện sau nextX second (ví dụ: 3s xuất hiện 1 note nhạc)
            if (nextX == -1) return;
            else
            {
                nextNote = Instantiate<GameObject>(Note[side]);
                nextNote.transform.position = new Vector3(
                    lastInsertWait[side].transform.position.x + nextX * speedNote *-Mathf.Pow(-1,side),
                    perfectLine[side].transform.position.y,
                    0
                );
            }
        }
        waitNote[side].Enqueue(nextNote);
        lastInsertWait[side] = nextNote;
    }
    //ấn vào nốt nhạc
    void ClickNote(int side)
    {
        //TODO: chưa xử lý perfect, early, later, miss, non-miss
        if(Physics2D.OverlapPoint(perfectLine[side].transform.position) !=null){
            Perfect(side);
        }else{
            nonMiss();
        }
    }
    //điểm
    void setScore(int score)
    {
        this.score = score;
        txt_score.text = ""+score;
    }
    //cập nhập vị trí note nhạc 
    void updatePosNote()
    {
        foreach (var item in waitNote[0])
            item.transform.position += new Vector3(+speedNote * Time.deltaTime, 0, 0);
        foreach (var item in waitNote[1])
            item.transform.position += new Vector3(-speedNote * Time.deltaTime, 0, 0);
    }
    //khi note nhạc đc chs hoàn hảo
    void Perfect(int side){
        showMSG("Perfect");
        GameObject.Destroy(waitNote[side].Dequeue());
        setScore(score + 100);
    }
    //khi note nhạc đc chs sớm 1 chút
    void Early(int side){

    }
    //khi note nhạc đc chs muộn 1 chút
    void Later(int side){

    }
    //khi ko bấm note nhạc 
    void Miss(int side){
        GameObject.Destroy(waitNote[side].Dequeue());
        showMSG("Miss");
    }
    //khi bấm nhưng ko có note nhạc nào
    void nonMiss(){

    }
    //Thua
    void Lose()
    {
        loadSound.pause();
        pause = true;
        canTouch = false;
        showMSG("You Lose!");
        // data.saveHighScore(score);
    }
    //thắng
    void Win()
    {
        pause = true;
        // loadSound.pause();
        showMSG("You Win!");
        // data.saveHighScore(score);
        canTouch = false;
    }
    //hiện thông báo
    void showMSG(string txt)
    {
        txt_msg.text=txt;
        Debug.Log(txt);
        // panel_msg.SetActive(true);
        // txt_msg.GetComponent<UnityEngine.UI.Text>().text = txt;
    }
}
