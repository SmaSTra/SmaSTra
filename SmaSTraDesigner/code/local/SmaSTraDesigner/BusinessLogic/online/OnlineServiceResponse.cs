
namespace SmaSTraDesigner.BusinessLogic.online
{
    public enum DownloadAllResponse
    {
        SUCCESS,
        FAILED
    }

    public enum DownloadSingleResponse
    {
        SUCCESS,
        FAILED_NO_NAME,
        FAILED_NAME_NOT_FOUND,
        FAILED_WHILE_EXTRACTING,
        FAILED_WHILE_LOADING
    }

    public enum UploadResponse
    {
        SUCCESS,
        FAILED_NO_NAME,
        FAILED_DUPLICATE_NAME
    }
}
