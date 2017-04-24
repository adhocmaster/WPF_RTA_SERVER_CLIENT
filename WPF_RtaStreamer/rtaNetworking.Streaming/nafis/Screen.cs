using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rtaNetworking.Streaming.nafis {
    public static class Screen {


        public static IEnumerable<Image> Snapshots() {
            return Screen.Snapshots( System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, true );
        }

        /// <summary>
        /// Returns a 
        /// </summary>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        public static IEnumerable<Image> Snapshots( int height, bool showCursor ) {

            Size size = new Size( System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height );

            Bitmap srcImage = new Bitmap( size.Width, size.Height );
            Graphics srcGraphics = Graphics.FromImage( srcImage );

            System.Diagnostics.Debug.WriteLine( "screen width : " + size.Width + " screen height : " + size.Height );

            // height is actually the resolution like, if height is 480 then it is 480p
            // we then get the screen ratio and adjust the width according to that
            //////////////////////////////////////

            double ratio = ( ( double ) size.Width / ( double ) size.Height );

            System.Diagnostics.Debug.WriteLine( "ratio : " + ratio );

            int width = ( int ) ( ratio * ( double ) height );

            //height = (int) ( width / ratio); //not necessary actually

            System.Diagnostics.Debug.WriteLine( "changed width : " + width + " changed height : " + height );

            /////////////////////////////////////


            bool scaled = ( width != size.Width || height != size.Height );

            Bitmap dstImage = srcImage;
            Graphics dstGraphics = srcGraphics;

            if ( scaled ) {
                dstImage = new Bitmap( width, height );
                dstGraphics = Graphics.FromImage( dstImage );
            }

            Rectangle src = new Rectangle( 0, 0, size.Width, size.Height );
            Rectangle dst = new Rectangle( 0, 0, width, height );
            Size curSize = new Size( 32, 32 );

            while ( true ) {
                srcGraphics.CopyFromScreen( 0, 0, 0, 0, size );

                if ( showCursor )
                    Cursors.Default.Draw( srcGraphics, new Rectangle( Cursor.Position, curSize ) );

                if ( scaled )
                    dstGraphics.DrawImage( srcImage, dst, src, GraphicsUnit.Pixel );

                yield return dstImage;

            }

            srcGraphics.Dispose();
            dstGraphics.Dispose();

            srcImage.Dispose();
            dstImage.Dispose();

            yield break;
        }

        internal static IEnumerable<MemoryStream> Streams( this IEnumerable<Image> source ) {
            MemoryStream ms = new MemoryStream();

            foreach ( var img in source ) {
                ms.SetLength( 0 );
                img.Save( ms, System.Drawing.Imaging.ImageFormat.Jpeg );
                yield return ms;
            }

            ms.Close();
            ms = null;

            yield break;
        }

    }
}
