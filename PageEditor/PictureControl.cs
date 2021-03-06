using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageEditor
{
    class PictureControl
    {
        private const int THUMBNAIL_SIZE = 20;

        internal class PictureInfo
        {
            public Image Picture;
            public Image ThumbImage;
            public DateTime Update;
        }

        /// <summary>
        /// Pictureファイルリスト
        /// </summary>
        private Dictionary<string, PictureInfo> PictureInfos = new Dictionary<string, PictureInfo>();

        /// <summary>
        /// 情報読み込み
        /// </summary>
        /// <param name="relativeFileName"></param>
        /// <returns></returns>
        internal PictureInfo Load(string relativeFileName)
        {
            // 引数異常
            if (relativeFileName == null)
                return null;

            // フルパスの取得
            string fullPath = MainForm.GetInstance().GetAbsolutePath(relativeFileName);

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
                pi.ThumbImage = CreateThumbnail(pi.Picture);

                PictureInfos[fullPath] = pi;

                return pi;
            }
            else
            {
                // 画像を読み込んで登録する
                PictureInfo pi = new PictureInfo();
                pi.Update = dateTime;
                pi.Picture = LoadPictureCore(fullPath);
                pi.ThumbImage = CreateThumbnail(pi.Picture);

                PictureInfos.Add(fullPath, pi);

                return pi;
            }
        }

        /// <summary>
        /// すでにOpen済みの画像ファイルを追加します。クリップボードから用
        /// </summary>
        /// <param name="relativeFileName"></param>
        /// <param name="image"></param>
        internal void Add(string fullPath, Image image)
        {
            DateTime dateTime = System.IO.File.GetLastWriteTime(fullPath);

            // 画像を登録する
            PictureInfo pi = new PictureInfo();
            pi.Update = dateTime;
            pi.Picture = image;
            pi.ThumbImage = CreateThumbnail(pi.Picture);

            PictureInfos.Add(fullPath, pi);
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
        /// サムネイル画像の生成
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static Image CreateThumbnail(Image image)
        {
            return image.GetThumbnailImage(THUMBNAIL_SIZE, THUMBNAIL_SIZE, delegate { return false; }, IntPtr.Zero);
        }
    }
}
