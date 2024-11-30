using BbxCommon.Ui;
using UnityEngine;

namespace BbxCommon
{
 
    public interface ILoadingProgress
    {
        public void OnLoading(float process);

        public void SetVisible(bool v);
        public void Close();
    }
}