using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageResample_with_Cs
{
      public  struct ResampledImage
    {
	public Bitmap image;
	public byte[] imagebytes;
    };
      public struct RectAngle
      {
          public int x1, x2, y1, y2;
      };
 
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(dlg.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            RawBitMap stc=new RawBitMap();
            pictureBox2.Image = stc.SetCenter((Bitmap)pictureBox1.Image);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RawBitMap RBMP = new RawBitMap();
            pictureBox3.Image = RBMP.Resample((Bitmap)pictureBox2.Image, 28, 28).image;
        }
    }




    //-----------------------------------------------------------------------------------
public class RawBitMap
{
    public ResampledImage Resample(Bitmap srcImg,int newWidth, int newHeight)
    {
	    int _width=srcImg.Size.Width;
	    int _height=srcImg.Size.Height;
	    byte[] _data = new byte[ _width*_height ];
	    int index=0;
        //set image to center of 90*90 recangle
        srcImg = SetCenter(srcImg);
        //convert source image into bytes
	    for (int x=0;x<_width;x++)
           {
		    for (int y=0;y<_height;y++)
		    {
			    if(srcImg.GetPixel(x,y).R==Color.White.R)
				    _data[index]=0;
			    else
				    _data[index]=1;
			    index++;
			
		    }
	    }
	    //resample the bytes of image
        byte[] newData = new byte [newWidth * newHeight];
        double scaleWidth =  (double)newWidth / (double)_width;
        double scaleHeight = (double)newHeight / (double)_height;
        for(int cy = 0; cy < newHeight; cy++)
        {
            for(int cx = 0; cx < newWidth; cx++)
            {
                int pixel = (cy * (newWidth )) + (cx);
                int nearestMatch =  (((int)(cy / scaleHeight) * (_width )) + ((int)(cx / scaleWidth)) );
                
                newData[pixel] =  _data[nearestMatch];
            }
        }
        //return resized image and its array
        ResampledImage rsi= new ResampledImage();
	    rsi.image=GetImage(newData,newWidth,newHeight);
        rsi.imagebytes = (byte[])newData.Clone();
        return rsi;
    }
    //set image to Center
    public Bitmap SetCenter(Bitmap image)
    {
        Bitmap bmp = new Bitmap(image.Size.Width, image.Size.Height);
        //get images data size
        RectAngle tmpRect = GetImageRectAngle(image);
            int tmpSizeW=tmpRect.x2 - tmpRect.x1;
            int tmpSizeH=tmpRect.y2 - tmpRect.y1;
            Bitmap tmpimg = new Bitmap(tmpSizeW,tmpSizeH);
        //copy the data from original image to tmpimg
        {

            for (int x = 0; x < tmpRect.x2 - tmpRect.x1; x++)
            {
                for (int y = 0; y < tmpRect.y2 - tmpRect.y1; y++)
                {
                    tmpimg.SetPixel(x, y, (image.GetPixel(tmpRect.x1 + x, tmpRect.y1 + y)));
                }
            }
        }
        //move tmpimg to center of image
        {
            int StartPointX=(image.Size.Width-tmpSizeW)/2;
            int StartPointY=(image.Size.Height-tmpSizeH)/2;
            for (int x = 0; x < image.Size.Width; x++)
            {
                for (int y = 0; y < image.Size.Height; y++)
                {
                    if (x > StartPointX && x < (StartPointX + tmpSizeW) &&
                        y > StartPointY && y < (StartPointY + tmpSizeH))
                    {
                        int tx,ty;
                        tx=x-StartPointX;
                        ty=y-StartPointY;
                        bmp.SetPixel(x, y, tmpimg.GetPixel(tx, ty));
                    }
                    else
                        bmp.SetPixel(x, y, Color.White);
                }
            }
        }
        return bmp;
    }
    //get the images data position and size
    public RectAngle GetImageRectAngle(Bitmap image)
    {
        RectAngle tmpRect = new RectAngle();
        int width = image.Size.Width;
        int height = image.Size.Height;
        //get x2
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (image.GetPixel(x, y).R == Color.Black.R)
                {
                    tmpRect.x2 = x;
                }

            }
        }
        //Get X1
        for (int x = width - 1; x > -1; x--)
        {
            for (int y = 0; y < height; y++)
            {
                if (image.GetPixel(x, y).R == Color.Black.R)
                {
                    tmpRect.x1 = x;
                }

            }
        }
        //Get Y2
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (image.GetPixel(x, y).R == Color.Black.R)
                {
                    tmpRect.y2 = y;
                }

            }
        }
        //Get Y1
        for (int y = height - 1; y > -1; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (image.GetPixel(x, y).R == Color.Black.R)
                {
                    tmpRect.y1 = y;
                }

            }
        }
        return tmpRect;
    }
    // Get image from its bytes array
    Bitmap GetImage(byte[] imagebytes,int w,int h)
    {
	    Bitmap tmp=new Bitmap(w,h);
	    int index=0;
	    for(int x=0;x<w;x++)
	    {
		    for (int y=0;y<h;y++)
		    {
			    if (imagebytes[index++]==0)
				    tmp.SetPixel(x,y,Color.White);
			    else
				    tmp.SetPixel(x,y,Color.Black);
		    }
	    }
        return tmp;
    }
};
    //---------------------------------------------------------------------------
}
