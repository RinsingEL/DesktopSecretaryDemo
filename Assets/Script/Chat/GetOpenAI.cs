using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class GetOpenAI : MonoBehaviour
{
    //配置参数
    [SerializeField]private PostData m_PostDataSetting;
	[System.Serializable]public class PostData{
		public string model;
		public string prompt;
		public int max_tokens; 
        public float temperature;
        public int top_p;
        public float frequency_penalty;
        public float presence_penalty;
        public string stop;
	}

	/// <summary>
	/// 返回的信息
	/// </summary>
	[System.Serializable]public class TextCallback{
		public string id;
		public string created;
		public string model;
		public List<TextSample> choices;

		[System.Serializable]public class TextSample{
			public string text;
			public string index;
			public string finish_reason;
		}

	}

}
