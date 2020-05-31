using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototipo6_ObtenerInfoPDF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string oldFile = "ejemploOK.pdf";
            string newFile = "temporal.pdf";
            PdfReader reader = new PdfReader(oldFile);
            IList<Dictionary<String, Object>> bmProperties = SimpleBookmark.GetBookmark(reader);
            PdfStamper stamp = new PdfStamper(reader, new FileStream(newFile, FileMode.Create));
            if (bmProperties != null)
            {
                listBox1.Items.Clear();
                MessageBox.Show("Cantidad marcadores " + bmProperties.Count());
                foreach (IDictionary<String, Object> bmProperty in bmProperties)
                {
                    //MessageBox.Show("Key " + bmProperty.Keys + " y values " + bmProperty.Values);
                    
                    //bmProperty.TryGetValue("Page", out valor);
                    
                    //foreach (var algo in bmProperty.Values)
                    //{
                        
                       /* if (algo == "Title")
                        {
                            listBox1.Items.Add(algo.);
                        }
                        if (algo == "Page")
                        {
                            listBox1.Items.Add(bmProperty.Values);
                        }*/
                        
                        MessageBox.Show("pagina " + bmProperty["Page"] + " - Titulo "+ bmProperty["Title"]);
                        var getPage = bmProperty["Page"];
                        var getTittle = bmProperty["Title"];
                       
                        var aux_pagina =getPage.ToString().Split(' ');
                        var pagina = aux_pagina[0];
                        var titulo = getTittle;
                        listBox1.Items.Add(titulo+ "  Pag. "+ pagina );
                       
                    //}
                }
                stamp.Close();
                reader.Close();            }
            else
            {
                MessageBox.Show("El documento No tiene marcadores / Se agregara uno a modo de prueba");
                IList<Dictionary<String, Object>> marcadores = new List<Dictionary<String, Object>>();
                
                //marcadores.Add(marcador);
                for (int i = 1; i <= 2; i++)
                {
                    Dictionary<String, Object> marcador = new Dictionary<String, Object>();
                    marcador.Add("Action", "GoTo");
                    marcador.Add("Title", "Marcador "+i);
                    marcador.Add("Page", i+" XYZ 100 600 0");
                    MessageBox.Show("marcador " + marcador.Count);
                    marcadores.Add(marcador);
                }
              

              

                stamp.Outlines = marcadores;
                stamp.Close();
                //System.Diagnostics.Process.Start(newFile);
                //Console.Read();
                reader.Close();
                File.Replace(newFile, oldFile, @"backup.pdf.bac");
                MessageBox.Show("Pdf modificado con exito, se ha guardado un backup de la versión anterior ");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap default_image = new Bitmap(pictureBox1.Image);
            Bitmap getIcono = new Bitmap("./noteIcon.png");
           
            Image<Bgr, Byte> fondo = new Image<Bgr, Byte>(default_image);
            Image<Bgr, Byte> fondo2 = new Image<Bgr, Byte>(default_image);
            Image <Bgr, Byte> icono = new Image<Bgr, Byte>(getIcono).Resize(0.8, Inter.Area);
            //MessageBox.Show("tamano icono " + icono.Width + " y " + icono.Height);
            string oldFile = "ejemploOK1.pdf";
            string newFile = "temporal.pdf";
            PdfReader reader = new PdfReader(oldFile);
            PdfDictionary pageDict = reader.GetPageN(1);
            
            
            

            iTextSharp.text.Rectangle pagesize = reader.GetPageSize(1);
            double anchoPDF = pagesize.Width;
            double altoPDF = pagesize.Height;
           // MessageBox.Show("Tamano pagina pdf --> ancho :" + anchoPDF + " y alto : " + altoPDF);
            float anchoImagen = fondo.Width;
            float altoImagen = fondo.Height;
            //MessageBox.Show("Tamano imagen --> ancho :" + anchoImagen+ " y alto : " + altoImagen);

            PdfArray annotArray = pageDict.GetAsArray(PdfName.ANNOTS);

            for (int i = 0; i < annotArray.Size; ++i)
            {
                PdfDictionary curAnnot = annotArray.GetAsDict(i);
                var subtype = curAnnot.Get(PdfName.SUBTYPE);
                var rect = curAnnot.Get(PdfName.RECT);
                var contents = curAnnot.Get(PdfName.CONTENTS);
                var page = curAnnot.Get(PdfName.P);
                //MessageBox.Show("Subtipo "+subtype + " coordenadas: "+rect + " y contenido "+contents);
                if (subtype.ToString() == "/Square")
                {
                    
                   
                    
                    //MessageBox.Show("Figura rectangular detectada coor "+rect);
                    var aux_rect = rect.ToString().Split(',');
                    aux_rect[0] = aux_rect[0].Remove(0, 1);
                    aux_rect[3] = aux_rect[3].Remove( aux_rect[3].Length-1);
                    aux_rect[0] = aux_rect[0].Replace(".", ",");
                    aux_rect[1] = aux_rect[1].Replace(".", ",");
                    aux_rect[2] = aux_rect[2].Replace(".", ",");
                    aux_rect[3] = aux_rect[3].Replace(".", ",");
                    //MessageBox.Show("el split primero " + aux_rect[0]+ " el split ultimo "+ aux_rect[3]);
                   // MessageBox.Show("Esto es rect " + aux_rect[0] + " " + aux_rect[1] + " " + aux_rect[2] + " " + aux_rect[3]);
                    int puntox1 = Convert.ToInt32(Convert.ToSingle(aux_rect[0]));
                    int puntoy1 = Convert.ToInt32(Convert.ToSingle(aux_rect[1]));
                    int puntox2 = Convert.ToInt32(Convert.ToSingle(aux_rect[2]));
                    int puntoy2 = Convert.ToInt32(Convert.ToSingle(aux_rect[3]));
                   // MessageBox.Show("puntos "+ puntox1+" ,"+ puntoy1+ " , "+ puntox2+" , "+ puntoy2);

                    // TRANSFORMAR COORDENADAS DESDE PDF
                    int nuevoX1 = (Convert.ToInt32(anchoImagen) * (puntox1 * 100 / Convert.ToInt32(anchoPDF))) / 100;
                    int nuevoX2 = (Convert.ToInt32(anchoImagen) * (puntox2 * 100 / Convert.ToInt32(anchoPDF))) / 100;
                    int nuevoY1 = Convert.ToInt32(altoImagen) - ((Convert.ToInt32(altoImagen) * (puntoy1 * 100 / Convert.ToInt32(altoPDF))) / 100);
                    int nuevoY2 = Convert.ToInt32(altoImagen)-((Convert.ToInt32(altoImagen) * (puntoy2 * 100 / Convert.ToInt32(altoPDF))) / 100);
                   // MessageBox.Show("puntos " + nuevoX1 + " ," + nuevoY1 + " , " + nuevoX2 + " , " + nuevoY2);

                    Rectangle rectangulo = new Rectangle(puntox1, puntoy1, puntox1+puntox2, puntoy1+puntoy2);
                    //Rectangle rectangulo = new Rectangle(0, 0, 100, 200);
                    fondo.Draw(rectangulo, new Bgr(Color.Cyan), 1);
                    Rectangle rectangulo2 = new Rectangle(nuevoX1, nuevoY1, nuevoX2-nuevoX1, nuevoY2-nuevoY1);
                    //Rectangle rectangulo = new Rectangle(0, 0, 100, 200);
                    fondo.Draw(rectangulo2, new Bgr(Color.Red), 1);
                    pictureBox1.Image = fondo.Bitmap;
                   // CvInvoke.cvResetImageROI(fondo);

                }
                if (subtype.ToString() == "/Text")
                {
                    //MessageBox.Show("Comentarios Post it detectado coor "+rect);
                    listBox2.Items.Add(contents+" "+page);
                    var aux_rect = rect.ToString().Split(',');
                    aux_rect[0] = aux_rect[0].Remove(0, 1);
                    aux_rect[0] = aux_rect[0].Replace(".", ",");
                    aux_rect[1] = aux_rect[1].Replace(".", ",");
                    int puntox1 = Convert.ToInt32(Convert.ToSingle(aux_rect[0]));
                    int puntoy1 = Convert.ToInt32(Convert.ToSingle(aux_rect[1]));
                    int nuevoX1 = (Convert.ToInt32(anchoImagen) * (puntox1 * 100 / Convert.ToInt32(anchoPDF))) / 100;
                    int nuevoY1 = Convert.ToInt32(altoImagen) - ((Convert.ToInt32(altoImagen) * (puntoy1 * 100 / Convert.ToInt32(altoPDF))) / 100);
                    //MessageBox.Show("Esto es punto x1 "+nuevoX1+ " y y1 "+nuevoY1);
                    //ROi debe ser del mismo tamaño que la imagen a pegar
                    Rectangle brect = new Rectangle(0, nuevoY1, icono.Width, icono.Height);
                    fondo.ROI = brect;
                    //final = auxImge.Copy();
                    CvInvoke.cvCopy(icono, fondo, IntPtr.Zero);
                    //pictureBox2.Image = fondo.Bitmap;


                    CvInvoke.cvResetImageROI(fondo);
                }
                
                //foreach (var algo in curAnnot.Keys)
                //{

                ////MessageBox.Show("Comentarios " + algo);
                // listBox2.Items.Add(algo);
                //}    
                //int someType = MyCodeToGetAnAnnotsType(curAnnot);
                //if (someType == THIS_TYPE)
                //{

                //}
                //else if (someType == THAT_TYPE)
                //{
                //writeThatType(curAnnot);
                //}
            }
        }
        

        public List<int> TransformarCoordenada(double anchoPDF, double altoPDF, int anchoImagen, int altoImagen, int puntox1, int puntoy1, int puntox2, int puntoy2)
        {
            List<int> nuevasCoor = new List<int>();
            int nuevoX1 = (Convert.ToInt32(anchoImagen) * (puntox1 * 100 / Convert.ToInt32(anchoPDF))) / 100;
            int nuevoX2 = (Convert.ToInt32(anchoImagen) * (puntox2 * 100 / Convert.ToInt32(anchoPDF))) / 100;
            int nuevoY1 = (Convert.ToInt32(altoImagen) * (puntoy1 * 100 / Convert.ToInt32(altoPDF))) / 100;
            int nuevoY2 = (Convert.ToInt32(altoImagen) * (puntoy1 * 100 / Convert.ToInt32(altoPDF))) / 100;
            
            return nuevasCoor;
        }
    }
}
