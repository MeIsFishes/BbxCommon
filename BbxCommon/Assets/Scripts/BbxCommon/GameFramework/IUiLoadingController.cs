namespace BbxCommon
{
    public interface IUiLoadingController
    {
        public void OnLoading(float process);

        public void SetVisible(bool v);
    }
}