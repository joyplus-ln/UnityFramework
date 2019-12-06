using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


    public class ExcelExporter : EditorWindow
    {
        private Dictionary<string, List<List<ICell>>> _sheets;
        private Vector2 m_SheetNamesScrollPos;
        private Vector2 m_SheetDataScrollPos;
        private float m_TopHeight = 80f;
        private float m_BottomHeight = 80f;
        private float m_LeftSheetWidth = 120f;
        private List<List<ICell>> m_CurSelectSheet;

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
        }

        protected void OnGUI()
        {
            // if (m_CurSelectSheet == null)
            // {
            // foreach (KeyValuePair<string, List<List<ICell>>> keyPair in sheets)
            // {
            //     m_CurSelectSheet = keyPair.Value;
            //     break;
            // }
            // }
            EditorUtils.Vertical(() =>
            {
                DrawDataAreaUI();
            });
        }
        private void DrawDataAreaUI()
        {
            EditorGUILayout.Space();

            GUIStyle helpBoxStyle = EditorStyleUtils.GetHelpBoxStyle();
            helpBoxStyle.richText = true;
            helpBoxStyle.fontSize = 15;

            EditorUtils.Horizontal(helpBoxStyle, () =>
            {
                EditorUtils.Vertical(helpBoxStyle, () => { DrawSheetNameList(); }, GUILayout.Width(m_LeftSheetWidth), GUILayout.ExpandHeight(true));
                EditorUtils.Vertical(helpBoxStyle, () => { DrawSheetList(); }, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            }, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        private void DrawSheetNameList()
        {
            EditorGUILayout.BeginVertical();
            {
                m_SheetNamesScrollPos = EditorGUILayout.BeginScrollView(m_SheetNamesScrollPos);
                GUI.color = Color.green;

                if (_sheets != null)
                {
                    foreach (KeyValuePair<string, List<List<ICell>>> keyPair in _sheets)
                    {
                        if (GUILayout.Button(keyPair.Key, EditorStyles.toolbarButton))
                        {
                            m_CurSelectSheet = keyPair.Value;
                        }
                    }
                }

                GUI.color = Color.white;
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSheetList()
        {
            EditorGUILayout.BeginVertical();
            {
                m_SheetDataScrollPos = EditorGUILayout.BeginScrollView(m_SheetDataScrollPos);
                if (m_CurSelectSheet != null)
                {
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < m_CurSelectSheet.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (i == 0)
                                {
                                    GUI.backgroundColor = Color.yellow;

                                    for (int j = 0; j < m_CurSelectSheet[i].Count; j++)
                                    {
                                        string str = m_CurSelectSheet[i][j] == null ? "" : m_CurSelectSheet[i][j].ToString();
                                        GUILayout.Box(str, GUILayout.Width(100), GUILayout.Height(30));
                                    }
                                }
                                else if (i == 1)
                                {
                                    GUI.backgroundColor = Color.gray;

                                    for (int j = 0; j < m_CurSelectSheet[i].Count; j++)
                                    {
                                        string str = m_CurSelectSheet[i][j] == null ? "" : m_CurSelectSheet[i][j].ToString();
                                        GUILayout.Box(str, GUILayout.Width(100), GUILayout.Height(30));
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < m_CurSelectSheet[i].Count; j++)
                                    {
                                        string str = m_CurSelectSheet[i][j] == null ? "" : m_CurSelectSheet[i][j].ToString();
                                        GUILayout.Box(str, GUILayout.Width(100), GUILayout.Height(30));
                                    }
                                }
                                GUI.backgroundColor = Color.white;
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        public Dictionary<string, List<List<ICell>>> sheets
        {
            set
            {
                _sheets = value;
                foreach (KeyValuePair<string, List<List<ICell>>> keyPair in _sheets)
                {
                    m_CurSelectSheet = keyPair.Value;
                    break;
                }
            }
        }
        // [UnityEditor.Callbacks.DidReloadScripts]
    }
