//  SceneUnit.cs
//  http://kan-kikuchi.hatenablog.com/entry/SceneUnit
//
//  Created by kan.kikuchi on 2017.09.16.

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 複数のシーンをまとめるクラス
/// </summary>
[Serializable]
public class SceneUnit{

  //名前
  [SerializeField]
  private string _name = "";

  //シーンのパスのリスト
  [SerializeField]
  private List<string> _pathList = new List<string>();

  //=================================================================================
  //初期化
  //=================================================================================

  /// <summary>
  /// コンストラクタ
  /// </summary>
  public SceneUnit(string name, List<string> pathList){
    _name     = name;
    _pathList = new List<string>(pathList);
  }

  //=================================================================================
  //読み込み
  //=================================================================================

  /// <summary>
  /// 設定されているシーンを読み込む
  /// </summary>
  public void Load(){
    //現在のシーンに変更があった場合、保存するか確認のウィンドウを出す(キャンセルされたら読み込みをしない)
    if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()){
      return;
    }

    //シーンが存在するかチェック
    for (int i = 0; i < _pathList.Count; i++) {
      string path = _pathList[i];
      if(AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null){
        Debug.LogError(path + "が存在しません！");
        return;
      }
    }

    //シーンを開く
    for (int i = 0; i < _pathList.Count; i++) {
      EditorSceneManager.OpenScene (_pathList[i], i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
    }
  }

  //=================================================================================
  //取得
  //=================================================================================

  /// <summary>
  /// 名前とパスをまとめたものを取得
  /// </summary>
  public string GetNameAndPath(){
    //シーンの名前をまとめて表示
    string nameAndPath = "";

    foreach (string path in _pathList) {
      if(!string.IsNullOrEmpty(nameAndPath)){
        nameAndPath += " + ";
      }

      nameAndPath += Path.GetFileNameWithoutExtension(path);
    }

    return _name + " : (" + nameAndPath + ")";
  }

}

/// <summary>
/// 複数のシーンをまとめるクラスをまとめたもの
/// </summary>
[Serializable]
public class SceneUnitSet{

  //インスタンス
  private static SceneUnitSet _instance;
  public  static SceneUnitSet  Instance {
    get {
      if (_instance == null) {

        //セーブデータのJsonを読み込み、データがあれば復元
        string json = EditorUserSettings.GetConfigValue(SAVE_KEY);

        if(string.IsNullOrEmpty(json)){
          _instance = new SceneUnitSet();
        }
        else{
          _instance = JsonUtility.FromJson<SceneUnitSet>(json);
        }

      }
      return _instance;
    }
  }

  //設定されたシーンユニットをまとめたList
  [SerializeField]
  private List<SceneUnit> _sceneUnitList = new List<SceneUnit>();

  //シーンユニットの数
  public int UnitNum{get{return _sceneUnitList.Count;}}

  //設定を保存する時のKey
  private const string SAVE_KEY = "SCENE_UNIT_SAVE_KEY";

  //=================================================================================
  //保存と読み込み
  //=================================================================================

  //SceneUnitSetの保存
  private void SaveSceneUnitList(){
    EditorUserSettings.SetConfigValue (SAVE_KEY, JsonUtility.ToJson(this));
  }

  //=================================================================================
  //追加、取得、削除
  //=================================================================================

  /// <summary>
  /// SceneUnitを作成し、追加
  /// </summary>
  public void Add(string sceneUnitName, List<string> scenePathList){
    //シーンユニットの名前が入力されていなければ、番号を設定
    if(string.IsNullOrEmpty(sceneUnitName)){
      sceneUnitName = UnitNum.ToString();
    }

    _sceneUnitList.Add(new SceneUnit(sceneUnitName, scenePathList));
    SaveSceneUnitList();
  }

  /// <summary>
  /// 指定した番号のSceneUnitを取得
  /// </summary>
  public SceneUnit GetAtNo(int no){
    return _sceneUnitList[no];
  }

  /// <summary>
  /// SceneUnitを削除
  /// </summary>
  public void Remove(SceneUnit sceneUnit){
    _sceneUnitList.Remove(sceneUnit);
    SaveSceneUnitList();
  }

  //=================================================================================
  //入れ替え
  //=================================================================================

  /// <summary>
  /// 要素を移動する
  /// </summary>
  public void Move(SceneUnit sceneUnit, bool isUp){
    int beforeNo = _sceneUnitList.IndexOf(sceneUnit);
    int afterNo  = beforeNo + (isUp ? - 1 : 1);

    _sceneUnitList[beforeNo] = _sceneUnitList[afterNo];
    _sceneUnitList[afterNo]  = sceneUnit;

    SaveSceneUnitList();
  }

}
