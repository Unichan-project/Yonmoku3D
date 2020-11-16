using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	struct hoge {
		public bool[] huga;
	}
	hoge hogehoge;
	List<hoge> hogeList = new List<hoge>();
    // Start is called before the first frame update
    void Start()
    {
		{
			hogehoge.huga = new bool[1];
			hogehoge.huga[0] = true;
			hogeList.Add(hogehoge);
			hogehoge.huga = new bool[1];
			hogehoge.huga[0] = false;
		}
		Debug.Log(hogeList[0].huga[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
