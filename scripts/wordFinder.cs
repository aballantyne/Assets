using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq; 
using System;
public class wordFinder : MonoBehaviour
{
    public static Dictionary<char,List<WordColecion>> wordDictionary; 

    public static List<char> blocked = new List<char>();
    public static Dictionary<char,List<int>> maybe = new Dictionary<char,List<int>>();
    public static Dictionary<char,List<int>> yes = new Dictionary<char,List<int>>();
    
    public static List<GameObject> buttton = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [MenuItem("Game/Setup")]
    static void MakeButtons(){
        
        GameObject prefab = Resources.Load<GameObject>("Button");
        GameObject canvas = GameObject.Find("Canvas");
        for (int y = 6-1; y >= 0; y--)
        {
            for (int x = 0; x < 5; x++)
            {
                GameObject newButton = Instantiate(prefab);
                newButton.name = x + ", " + y;
                newButton.transform.parent = canvas.transform; 
                newButton.transform.position = new Vector2(x*1.2f-2.5f,y*1.2F-3); 
            }
        }
    }
    [MenuItem("Tools/Find wordle")]
    static void Test(){
        blocked = new List<char>();
        maybe= new Dictionary<char,List<int>>();
        yes= new Dictionary<char,List<int>>();

        blocked.Add('s');        
        blocked.Add('r');        
        blocked.Add('e');   
        blocked.Add('i');        
        blocked.Add('n');        
        blocked.Add('u');        
        blocked.Add('d');        




        yes.Add('a', new List<int>(){0});
        yes.Add('o', new List<int>(){2});
        yes.Add('l', new List<int>(){1});


        //maybe.Add('o', new List<int>(){3,0});
        
        
        
        if (wordDictionary == null)CreateDictionary();

        string bestWord = String.Empty;
        int bestFitness = -Int32.MaxValue; 
        foreach(var entry in wordDictionary)
        {
            
                foreach(WordColecion item in entry.Value){
                    if (item.fitness > bestFitness && !Contains(blocked,item.wordString) && MaybeCheck(maybe,item.wordString) && YesCheck(yes,item.wordString)){
                        bestWord = item.wordString;
                        bestFitness = item.fitness;
                    }
                    
                }
            
           
        }
        if (bestWord != String.Empty){
            Debug.Log(bestWord);
        }else {
            Debug.Log("Oops no word found check the text");
        }
    }
    static bool MaybeCheck(Dictionary<char,List<int>> dic, string item){
        if (dic.Count == 0) return true;
        foreach (KeyValuePair<char,List<int>> kvp in dic){
            if(item.Contains(kvp.Key)){
                char[] array = item.ToCharArray();
                bool wordBool = false;
                foreach (int index in kvp.Value){
                    if(array[index] == kvp.Key)wordBool = true;
                }
                if(!wordBool) return true;
            }else {
                return false;
            }
        }
        return false;
    }    
    static bool YesCheck(Dictionary<char,List<int>> dic, string item){
        if (dic.Count == 0) return true;
        foreach (KeyValuePair<char,List<int>> kvp in dic){
            if(item.Contains(kvp.Key)){
                char[] array = item.ToCharArray();
                bool wordBool = false;
                foreach (int index in kvp.Value){
                    if(array[index] == kvp.Key)wordBool = true;
                }
                if(!wordBool) return false;
            }else {
                return false;
            }
        }
        return true;
    }
    static bool Contains( List<char> list, string item){
        for (int i = 0; i<list.Count; i++){
            if (item.Contains(list[i])) return true;
        }
        return false; 
    }
    [MenuItem("Tools/Load wordle dictionary")]
    static void CreateDictionary(){
        wordDictionary = new Dictionary<char,List<WordColecion>>(); 
        
        string path = "Assets/Resources/words_alpha.txt";
        StreamReader reader = new StreamReader(path); 
        string[] words = reader.ReadToEnd().Split('\n');
        
        char[] alphabet = "qxjzvfwkbghcpmyduntliroaes".ToCharArray();
       
        for(int i = 0; i<words.Length; i++)
        { 
            for (int letter = 26 - 1; letter >= 0 ; letter--)
            {
                if (words[i].Contains(alphabet[letter])){
                    if (!wordDictionary.ContainsKey(alphabet[letter])){
                        wordDictionary.Add(alphabet[letter], new List<WordColecion>());
                    }
                    
                    wordDictionary[alphabet[letter]].Add(new WordColecion(words[i]));
                }
            }
        }   
    }
    [MenuItem("Tools/Load alphabet")]
    static void CreateLetterValues(){
        char[] alphabet = "qxjzvfwkbghcpmyduntliroaes".ToCharArray();

        string path = "Assets/Resources/words_alpha.txt";
        StreamReader reader = new StreamReader(path); 
        string[] words = reader.ReadToEnd().Split('\n');
        
        Dictionary<char,int> letterValue = new Dictionary<char,int>();
        foreach (char letter in alphabet)
        {
            foreach (string item in words){
                if (item.Contains(letter)){
                    if (!letterValue.ContainsKey(letter)){
                        letterValue.Add(letter, 0);
                    }
                    letterValue[letter]++;
                }
            }
        }
        string output = String.Empty;
        string number = String.Empty;
        foreach (KeyValuePair<char, int> letter in letterValue.OrderBy(key => key.Value))  
        {  
            output += letter.Value+ ",";
            number += letter.Key ;
        } 
        Debug.Log(output);
        Debug.Log(number);

    }
}
public class WordColecion {
    public string wordString;
    public int fitness; 

    public char[] alphabet;

    public WordColecion (string WORDSTRING){
        wordString = WORDSTRING;

        alphabet = "qxjzvfwkbghcpmyduntliroaes".ToCharArray();

        for (int letter = 0; letter < 26 ; letter++){
            int count = wordString.Count(f => f == alphabet[letter]);
            if (count == 1){
                fitness += letter; 
            }else if (count > 1){
                fitness -= letter;
            }
        }
        
        
    }
}
