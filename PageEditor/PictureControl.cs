using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageEditor
{
    static class PictureControl
    {
        class PictureInfo
        {
            public Image Picture;
            public Image ThumbImage;
            public DateTime Update;
        }

        /// <summary>
        /// Pictureファイルリスト
        /// </summary>
        static private Dictionary<string, PictureInfo> PictureInfos = new Dictionary<string, PictureInfo>();

        /// <summary>
        /// Imageの取得
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Image Load(string relativeFileName)
        {
            PictureInfo pi = LoadInfo(relativeFileName);

            if (pi == null)
                return null;

            return pi.Picture;
        }

        /// <summary>
        /// サムネイルの取得
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static Image GetThumbImage(string relativeFileName)
        {
            PictureInfo pi = LoadInfo(relativeFileName);

            if (pi == null)
                return null;

            return pi.ThumbImage;
        }

        private static PictureInfo LoadInfo(string relativeFileName)
        {
            // 引数異常
            if (relativeFileName == null)
                return null;

            // フルパスの取得
            string fullPath = MainForm.GetAbsolute(relativeFileName);

            // ファイルがない場合はここで終わり
            if (System.IO.File.Exists(fullPath) == false)
            {
                // 存在する場合
                if (PictureInfos.ContainsKey(fullPath))
                {
                    // 消しておく
                    PictureInfos.Remove(fullPath);
                }

                return null;
            }

            // 最終更新日時の取得
            DateTime dateTime = System.IO.File.GetLastWriteTime(fullPath);

            // 辞書に存在する場合
            if (PictureInfos.ContainsKey(fullPath))
            {
                // 辞書から取得
                PictureInfo pi = PictureInfos[fullPath];

                // 更新されていない場合
                if (pi.Update == dateTime)
                {
                    // そのまま返す
                    return pi;
                }

                // データ更新して詰めなおす。
                pi.Update = dateTime;
                pi.Picture = LoadPictureCore(fullPath);
                pi.ThumbImage = pi.Picture.GetThumbnailImage(18, 18, delegate { return false; }, IntPtr.Zero);

                PictureInfos[fullPath] = pi;

                return pi;
            }
            else
            {
                // 画像を読み込んで登録する
                PictureInfo pi = new PictureInfo();
                pi.Update = dateTime;
                pi.Picture = LoadPictureCore(fullPath);
                pi.ThumbImage = pi.Picture.GetThumbnailImage(18, 18, delegate { return false; }, IntPtr.Zero);

                PictureInfos.Add(fullPath, pi);

                return pi;
            }
        }

        /// <summary>
        /// Lockしない画像読み込み
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private static Image LoadPictureCore(string fullPath)
        {
            try
            {
                Image img = null;
                using (FileStream fs = new FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    img = Image.FromStream(fs);
                    fs.Close();
                }
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// すでにOpen済みの画像ファイルを追加します。クリップボードから用
        /// </summary>
        /// <param name="relativeFileName"></param>
        /// <param name="image"></param>
        internal static void Add(string fullPath, Image image)
        {
            DateTime dateTime = System.IO.File.GetLastWriteTime(fullPath);

            // 画像を登録する
            PictureInfo pi = new PictureInfo()
            {
                Picture = image,
                Update = dateTime
            };

            PictureInfos.Add(fullPath, pi);
        }
    }
}
