using System;
using System.Collections;
using System.Collections.Generic;
using FastFrameWork;
using UnityEngine;

public class GameManagers
{
   private List<BaseManager> _baseManagers = new List<BaseManager>();
   public Action IninFinished = null;

   private void LoadFrameworkManagers()
   {
      BindManager<Timer>();
      BindManager<MemoryManager>();
   }
   /// <summary>
   /// 初始化
   /// </summary>
   /// <returns></returns>
   public void Init(Action IninFinished)
   {
      LoadFrameworkManagers();
      this.IninFinished = IninFinished;
      for (int i = 0; i < _baseManagers.Count; i++)
      {
         _baseManagers[i].Init();
      }

      InitAsync();
   }

   private void InitAsync()
   {
      for (int i = 0; i < _baseManagers.Count; i++)
      {
         if ( !_baseManagers[i].Finished())
         {
            _baseManagers[i].InitFinshed = InitAsync;
            _baseManagers[i].InitAsync();
            break;
         }
         IninFinished?.Invoke();
      }
   }

   /// <summary>
   /// 绑定一个manager
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public void BindManager<T>() where T:BaseManager
   {
      _baseManagers.Add(Activator.CreateInstance<T>());
   }
   
   public void Update()
   {
      for (int i = 0; i < _baseManagers.Count; i++)
      {
         _baseManagers[i].Update();
      }
   }

   public void GamePause(bool pause)
   {
      for (int i = 0; i < _baseManagers.Count; i++)
      {
         _baseManagers[i].GamePause(pause);
      }
   }

   public void ExitGame()
   {
      for (int i = 0; i < _baseManagers.Count; i++)
      {
         _baseManagers[i].ExitGame();
      }
   }
}
