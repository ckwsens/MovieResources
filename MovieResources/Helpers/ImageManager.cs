namespace MovieResources.Helpers
{
    public class ImageManager
    {
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="path">图片地址</param>
        public static void Delete(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}