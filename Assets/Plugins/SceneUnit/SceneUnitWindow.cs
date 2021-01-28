//  SceneUnitWindow.cs
//  http://kan-kikuchi.hatenablog.com/entry/SceneUnit
//
//  Created by kan.kikuchi on 2017.09.16.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 複数のシーンの一度に開くためのウィンドウ
/// </summary>
public class SceneUnitWindow : EditorWindow {

  //スクロール位置
  private Vector2 _scrollPosition = Vector2.zero;

  //プロジェクト内の全シーンのパスと、それが選択されているか
  private Dictionary<string, bool> _scenePathDict = new Dictionary<string, bool>();

  //選択中のシーンのパスのList
  private List<string> _selectingScenePathList = new List<string>();

  //シーンユニットの名前
  private string _sceneUnitName = "";

  //=================================================================================
  //ウィンドウ表示
  //=================================================================================

  //メニューからウィンドウを表示
  [MenuItem("Window/SceneUnitWindow")]
  public static void Open (){
    SceneUnitWindow.GetWindow<SceneUnitWindow>(typeof(SceneUnitWindow));
  }

  //=================================================================================
  //初期化
  //=================================================================================

  //初期化
  private void Init(){
    _scenePathDict = AssetDatabase.FindAssets("t:SceneAsset") //シーンのアセットのGUIDを取得
      .Select(guid => AssetDatabase.GUIDToAssetPath(guid))    //GUIDからパスに変換
      .ToDictionary(path => path, flag => false);             //パスをKey、選択中かのフラグをfalseにしてDictonary作成

    _sceneUnitName = "";
    _selectingScenePathList.Clear();
  }

  private void OnEnable(){
    Init();
  }

  //=================================================================================
  //表示するGUIの設定
  //=================================================================================

  private void OnGUI(){
    //描画範囲が足りなければスクロール出来るように
    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUI.skin.scrollView);

    EditorGUILayout.BeginVertical(GUI.skin.box);{
      if(SceneUnitSet.Instance.UnitNum != 0){
        OnGUIWithTitle(OnSettingSceneGUI, "設定したシーンユニット");
      }
      OnGUIWithTitle(OnAllSceneGUI, "プロジェクト内のシーン");
      if(_selectingScenePathList.Count != 0){
        OnGUIWithTitle(OnSelectingSceneGUI, "選択中のシーン");
      }
    }EditorGUILayout.EndVertical();

    EditorGUILayout.EndScrollView();
  }

  //装飾とタイトルを付けて、GUIを表示
  private void OnGUIWithTitle(Action onGUIAction, string title){
    EditorGUILayout.BeginVertical(GUI.skin.box);{

      EditorGUILayout.LabelField(title);
      GUILayout.Space(10);
      onGUIAction();

    }EditorGUILayout.EndVertical();

    GUILayout.Space (10);
  }

  //設定したシーン一覧を表示するGUIの設定
  private void OnSettingSceneGUI(){

    for (int i = 0; i < SceneUnitSet.Instance.UnitNum; i++) {
      EditorGUILayout.BeginHorizontal (GUI.skin.box);

      //削除ボタン表示
      if (GUILayout.Button ("×", GUILayout.Width (20))) {
        SceneUnitSet.Instance.Remove(SceneUnitSet.Instance.GetAtNo(i));
        return;
      }

      //読み込みボタン表示
      EditorGUILayout.LabelField (SceneUnitSet.Instance.GetAtNo(i).GetNameAndPath());
      if (GUILayout.Button ("読み込み", GUILayout.Width (100))) {
        SceneUnitSet.Instance.GetAtNo(i).Load();
        return;
      }

      //上下ボタン表示
      if (i > 0) {
        if (GUILayout.Button ("↑", GUILayout.Width (20))) {
          SceneUnitSet.Instance.Move(SceneUnitSet.Instance.GetAtNo(i), isUp:true);
          return;
        }
      }
      else {
        GUILayout.Label ("", GUILayout.Width (20));
      }

      if(i < SceneUnitSet.Instance.UnitNum - 1) {
        if (GUILayout.Button ("↓", GUILayout.Width (20))) {
          SceneUnitSet.Instance.Move(SceneUnitSet.Instance.GetAtNo(i), isUp:false);
          return;
        }
      }
      else {
        GUILayout.Label ("", GUILayout.Width (20));
      }
      EditorGUILayout.EndHorizontal ();
    }

  }

  //プロジェクト内の全シーンを表示するのGUIの設定
  private void OnAllSceneGUI(){
    //全シーンのパスを表示
    List<string> changedPathList = new List<string>();

    foreach (KeyValuePair<string, bool> pair in _scenePathDict) {
      EditorGUILayout.BeginHorizontal (GUI.skin.box);

      bool beforeFlag = pair.Value;
      bool afterFlag  = EditorGUILayout.ToggleLeft(Path.GetFileNameWithoutExtension(pair.Key), beforeFlag);

      //チェックボックスの変更があればListに登録
      if(beforeFlag != afterFlag){
        changedPathList.Add(pair.Key);
      }

      //読み込みボタン表示
      if(GUILayout.Button ("読み込み", GUILayout.Width (100))) {
        //現在のシーンに変更があった場合、保存するか確認のウィンドウを出す(キャンセルされたら読み込みをしない)
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()){
          return;
        }

        //シーンが存在するかチェック
        if(AssetDatabase.LoadAssetAtPath<SceneAsset>(pair.Key) == null){
          Debug.LogError(pair.Key + "が存在しません！");
          return;
        }

        //シーン読み込み
        EditorSceneManager.OpenScene (pair.Key);
        return;
      }

      EditorGUILayout.EndHorizontal ();
    }

    //変更があったパスのフラグを変更、選択中のシーンのListも更新
    foreach (string changedPath in changedPathList) {
      _scenePathDict[changedPath] = !_scenePathDict[changedPath];

      if(_scenePathDict[changedPath]){
        _selectingScenePathList.Add(changedPath);
      }
      else{
        _selectingScenePathList.Remove(changedPath);
      }
    }
    GUILayout.Space (10);

    //シーン再取得と選択全解除を行うボタン表示
    if(GUILayout.Button ("シーン再取得、選択全解除")){
      Init();
    }
  }

  //選択中のシーンを表示するGUI
  private void OnSelectingSceneGUI(){
    //選択中のシーンを表示
    EditorGUILayout.BeginVertical(GUI.skin.box);{

      for (int i = 0; i < _selectingScenePathList.Count; i++) {
        string path = _selectingScenePathList[i];
        EditorGUILayout.LabelField((i + 1).ToString() + " : " + Path.GetFileNameWithoutExtension(path));
      }

    }EditorGUILayout.EndVertical();
    GUILayout.Space (10);

    //シーンユニットの名前を入力するGUIを表示
    _sceneUnitName = EditorGUILayout.TextField("シーンユニット名", _sceneUnitName);
    GUILayout.Space (10);

    //シーンユニットの設定を行うボタン表示
    if(GUILayout.Button ("設定")){
      SceneUnitSet.Instance.Add(_sceneUnitName, _selectingScenePathList);
      Init();
    }
  }

}
