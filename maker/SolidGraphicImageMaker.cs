

using halba.imageditor.bitmap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace halba.imageditor.maker
{
    public class SolidGraphicImageMaker
    {
        public static Bitmap CreateGlobalPng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                //gfx.SmoothingMode     = SmoothingMode.None;
                //gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.Half;

                int depth_center = (int)Math.Round((double)pen_depth / 2.0);
                int height_comp = width_height - pen_depth;
                int center = (int)Math.Round((double)width_height / 2.0);
                int center_line_h = (int)Math.Round((double)width_height * 0.48);

                gfx.Clear(Color.Transparent);

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);
                Rectangle rectc = new Rectangle(depth_center, depth_center, height_comp, height_comp);
                gfx.DrawEllipse(pen, rectc);

                gfx.DrawLine(pen, new PointF(0, center), new PointF(width_height, center));
                gfx.DrawLine(pen, new PointF(center, 0), new PointF(center, width_height));

                int line_center_pos = center - (int)Math.Round((double)center_line_h / 2.0);
                Rectangle recta = new Rectangle(line_center_pos, depth_center, center_line_h, height_comp);
                gfx.DrawArc(pen, recta, 0, 360);

                int arc_down_pos = (int)Math.Round((double)width_height * 0.725);
                int arc_top_pos = (int)Math.Round((double)width_height * 0.275);
                int arc_height = (int)Math.Round((double)width_height * 0.54);

                Rectangle recta2 = new Rectangle(0, arc_down_pos, width_height, arc_height);
                gfx.DrawArc(pen, recta2, 205, 130);

                Rectangle recta3 = new Rectangle(0, -arc_top_pos, width_height, arc_height);
                gfx.DrawArc(pen, recta3, 25, 130);

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateParamPng(float width_height, Color pen_color, float pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap((int)width_height, (int)width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);
                float padding = 1;

                float dragger = width_height * 0.26f;
                float dra_center = (dragger / 2.0f);
                float center = width_height / 2.0f;
                float motion = (dragger / 2.0f);
                float center_pos = center - dra_center;
                float top_pos1 = motion * 1.4f;
                float top_pos3 = motion;
                float down_pos = width_height - dragger - motion;
                float rigth_pos = width_height - dragger - pen_depth;

                Rectangle rectc1 = new Rectangle((int)Math.Round(pen_depth), (int)Math.Round(top_pos1), (int)Math.Round(dragger), (int)Math.Round(dragger));
                Rectangle rectc2 = new Rectangle((int)Math.Round(center_pos), (int)Math.Round(down_pos), (int)Math.Round(dragger), (int)Math.Round(dragger));
                Rectangle rectc3 = new Rectangle((int)Math.Round(rigth_pos), (int)Math.Round(top_pos3), (int)Math.Round(dragger), (int)Math.Round(dragger));

                gfx.DrawEllipse(pen, rectc1);
                gfx.DrawEllipse(pen, rectc2);
                gfx.DrawEllipse(pen, rectc3);

                // Passos 01
                float _011_top_x = dra_center + pen_depth;
                float _011_top_y = padding;
                float _011_down_x = _011_top_x;
                float _011_down_y = top_pos1;
                gfx.DrawLine(pen, new PointF(_011_top_x, _011_top_y), new PointF(_011_down_x, _011_down_y));

                float _012_top_x = dra_center + pen_depth;
                float _012_top_y = top_pos1 + dragger;
                float _012_down_x = _012_top_x;
                float _012_down_y = width_height - padding;
                gfx.DrawLine(pen, new PointF(_012_top_x, _012_top_y), new PointF(_012_down_x, _012_down_y));


                // Passos 02
                float _021_top_x = center;
                float _021_top_y = _011_top_y;
                float _021_down_x = _021_top_x;
                float _021_down_y = down_pos;
                gfx.DrawLine(pen, new PointF(_021_top_x, _021_top_y), new PointF(_021_down_x, _021_down_y));

                float _022_top_x = center;
                float _022_top_y = down_pos + dragger;
                float _022_down_x = _021_top_x;
                float _022_down_y = width_height - padding;
                gfx.DrawLine(pen, new PointF(_022_top_x, _022_top_y), new PointF(_022_down_x, _022_down_y));

                // Passos 03
                float _031_top_x = width_height - dra_center - pen_depth;
                float _031_top_y = _011_top_y;
                float _031_down_x = _031_top_x;
                float _031_down_y = top_pos3;
                gfx.DrawLine(pen, new PointF(_031_top_x, _031_top_y), new PointF(_031_down_x, _031_down_y));

                float _033_top_x = _031_top_x;
                float _033_top_y = top_pos3 + dragger;
                float _033_down_x = _033_top_x;
                float _033_down_y = width_height - padding;
                gfx.DrawLine(pen, new PointF(_033_top_x, _033_top_y), new PointF(_033_down_x, _033_down_y));
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateRightStemlessShortArrowPng(int height, Color pen_color, int pen_depth)
        {
            int height_output = height;
            bool restore = false;

            if (height < 100)
            {
                height = 100;
                restore = true;
            }

            int width = (int)Math.Round((double)height * 0.5);

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);

                int center = (int)Math.Round((double)height * 0.5);

                gfx.DrawLine(pen, new Point(0, 0), new Point(width, center + pen_depth));
                gfx.DrawLine(pen, new Point(width, center - pen_depth), new Point(0, height));
            }

            if (restore)
            {
                int w = (int)Math.Round((double)height_output * 0.5);
                return BitmapEditor.ResizeImage(bmp, w, height_output);
            }
            return bmp;
        }


        public static Bitmap CreateExpandPng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);

                int depth_center = (int)Math.Round((double)pen_depth / 2.0);
                int center       = (int)Math.Round((double)width_height * 0.5);
                int square_size  = (int)Math.Round((double)width_height * 0.2);
                int donw_square  = width_height - square_size - pen_depth;
                int arrow_leng   = (int)Math.Round((double)width_height * 0.2);

                gfx.DrawLine(pen, new Point(0, depth_center), new Point(width_height - pen_depth, depth_center));
                gfx.DrawLine(pen, new Point(0, width_height - pen_depth), new Point(width_height - pen_depth, width_height - pen_depth));

                gfx.DrawLine(pen, new Point(depth_center, 0), new Point(depth_center, square_size));
                gfx.DrawLine(pen, new Point(width_height - pen_depth, 0), new Point(width_height - pen_depth, square_size));

                gfx.DrawLine(pen, new Point(depth_center, donw_square), new Point(depth_center, (width_height - pen_depth)));
                gfx.DrawLine(pen, new Point(width_height - pen_depth, donw_square), new Point(width_height - pen_depth, (width_height - depth_center)));


                // Arrow Top
                int arrow_top_l_p1_x = center - arrow_leng;
                int arrow_top_l_p1_y = square_size + arrow_leng - pen_depth;
                int arrow_top_l_p2_x = center;
                int arrow_top_l_p2_y = square_size;
                int arrow_top_r_p1_x = center - depth_center;
                int arrow_top_r_p1_y = square_size;
                int arrow_top_r_p2_x = center + arrow_leng - depth_center;
                int arrow_top_r_p2_y = arrow_top_l_p1_y;

                // Teste. Todos tem que ser iguais ABS
                int x1 = arrow_top_l_p1_x - arrow_top_l_p2_x;
                int y1 = arrow_top_l_p1_y - arrow_top_l_p2_y;
                int x2 = arrow_top_r_p1_x - arrow_top_r_p2_x;
                int y2 = arrow_top_r_p1_y - arrow_top_r_p2_y;

                gfx.DrawLine(pen, new Point(arrow_top_l_p1_x, arrow_top_l_p1_y), new Point(arrow_top_l_p2_x, arrow_top_l_p2_y));
                gfx.DrawLine(pen, new Point(arrow_top_r_p1_x, arrow_top_r_p1_y), new Point(arrow_top_r_p2_x, arrow_top_r_p2_y));

                // Arrow Down
                int arrow_down_l_p1_x = arrow_top_l_p1_x;
                int arrow_down_l_p1_y = width_height - arrow_top_l_p1_y - pen_depth;
                int arrow_down_l_p2_x = arrow_top_l_p2_x;
                int arrow_down_l_p2_y = width_height - arrow_top_l_p2_y - pen_depth;
                int arrow_down_r_p1_x = arrow_top_r_p1_x;
                int arrow_down_r_p1_y = width_height - arrow_top_r_p1_y - pen_depth;
                int arrow_down_r_p2_x = arrow_top_r_p2_x;
                int arrow_down_r_p2_y = width_height - arrow_top_r_p2_y - pen_depth;

                // Teste. Todos tem que ser iguais ABS
                int x3 = arrow_down_l_p1_x - arrow_down_l_p2_x;
                int y3 = arrow_down_l_p1_y - arrow_down_l_p2_y;
                int x4 = arrow_down_r_p1_x - arrow_down_r_p2_x;
                int y4 = arrow_down_r_p1_y - arrow_down_r_p2_y;

                gfx.DrawLine(pen, new Point(arrow_down_l_p1_x, arrow_down_l_p1_y), new Point(arrow_down_l_p2_x, arrow_down_l_p2_y));
                gfx.DrawLine(pen, new Point(arrow_down_r_p1_x, arrow_down_r_p1_y), new Point(arrow_down_r_p2_x, arrow_down_r_p2_y));
                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateMinusPng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);

                int center = (int)Math.Round((double)width_height * 0.5);
                gfx.DrawLine(pen, new Point(0, center), new Point(width_height, center));

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateLogoutPng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                int square_xsize = (int)Math.Round((double)width_height * 0.4);
                int center = (int)Math.Round((double)width_height * 0.5);
                int coffset = (int)Math.Round((double)width_height * 0.35);
                int arrow_size = (int)Math.Round((double)coffset / 1.3);
                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);

                // Half Square
                gfx.DrawLine(pen, new Point(0, 0), new Point(0, width_height));
                gfx.DrawLine(pen, new Point(0, 0), new Point(square_xsize, 0));
                gfx.DrawLine(pen, new Point(0, width_height - pen_depth), new Point(square_xsize, width_height - pen_depth));

                // Arrow
                gfx.DrawLine(pen, new Point(coffset, center), new Point(width_height, center));
                gfx.DrawLine(pen, new Point((width_height - arrow_size) - pen_depth, (center - arrow_size)), new Point(width_height - pen_depth, center));
                gfx.DrawLine(pen, new Point((width_height - arrow_size) - pen_depth, (center + arrow_size)), new Point(width_height - pen_depth, center));

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateRestoredPng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                int depth_center = (int)Math.Round((double)pen_depth / 2.0);

                int square_size = (int)Math.Round((double)width_height * 0.73);
                int r1_y = width_height - (int)square_size - pen_depth;

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);
                Rectangle r1 = new Rectangle(depth_center, r1_y, square_size, square_size);
                gfx.DrawRectangle(pen, r1);
                gfx.DrawLine(pen, new Point(r1_y, depth_center), new Point(width_height - depth_center, depth_center));
                gfx.DrawLine(pen, new Point((width_height - pen_depth), 0), new Point((width_height - pen_depth), square_size));
                gfx.DrawLine(pen, new Point((width_height - pen_depth), square_size - depth_center), new Point((width_height - r1_y - pen_depth), square_size - depth_center));
                gfx.DrawLine(pen, new Point(r1_y, 0), new Point(r1_y, r1_y));

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreatePersonPng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                var head_size = (((double)width_height / 2.0) * 1.2);
                int csize = (int)Math.Round(head_size);
                int start = (int)Math.Round(((double)width_height - head_size) / 2.0);

                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);
                Rectangle rectc = new Rectangle(start, 0, csize, csize);
                gfx.DrawEllipse(pen, rectc);

                Rectangle recta = new Rectangle(0, csize, width_height, width_height);
                gfx.DrawArc(pen, recta, 180, 180);

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateClosePng(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                SolidBrush pen = new SolidBrush(pen_color);

                int ix = 0;
                while (ix < width_height)
                {
                    Rectangle p = new Rectangle(ix, ix, pen_depth, pen_depth);
                    gfx.FillRectangle(pen, p);
                    ix++;
                }

                var offset = pen_depth > 1 ? pen_depth - 1 : 0;
                int iy = ix - offset;
                ix = 0;
                while (iy > 0)
                {
                    iy--;
                    Rectangle p = new Rectangle(ix, iy, pen_depth, pen_depth);
                    gfx.FillRectangle(pen, p);
                    ix++;
                }

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


        public static Bitmap CreateGear(int width_height, Color pen_color, int pen_depth)
        {
            int width_height_output = (int)width_height;
            bool restore = false;

            if (width_height < 100)
            {
                width_height = 100;
                restore = true;
            }

            Bitmap bmp = new Bitmap(width_height, width_height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.Clear(Color.Transparent);

                var head_size = (((double)width_height / 2.0) * 1.2);
                int csize = (int)Math.Round(head_size);
                int start = (int)Math.Round(((double)width_height - head_size) / 2.0);
                Pen pen = new Pen(new SolidBrush(pen_color), pen_depth);

                int centerc = (int)Math.Round((double)width_height * 0.25);
                int circlec = (int)Math.Round((double)width_height / 2.0);
                DrawCircle(gfx, pen, centerc, circlec, circlec, 36);

                int center = (int)Math.Round((double)width_height * 0.5);
                DrawGear(gfx, pen, center, center, center, 160, 6, 22, 0.3, 0.27);

                pen.Dispose();
            }

            if (restore)
            {
                return BitmapEditor.ResizeImage(bmp, width_height_output, width_height_output);
            }
            return bmp;
        }


       

        public static void DrawGear(Graphics gfx, Pen pen, double radius, double center_x, double center_y, double resolution, int number_tooths, double tooths_size, double tooth_depth, double side_angle)
        {
            double TOTAL_POR_DENTES         = tooths_size * (double)number_tooths;
            double TOTAL_DESLOCAMENTO       = side_angle * (double)number_tooths;
            double TOTAL_BASE_DENTE         = TOTAL_POR_DENTES * TOTAL_DESLOCAMENTO;
            double SOBRA_BASE               = 360.0 - TOTAL_BASE_DENTE;
            double BASE_POR_DENTE           = SOBRA_BASE / (double)number_tooths;

            double CENTRO_BASE_POR_DENTE    = ToRadians(BASE_POR_DENTE / 2.0);
            double CENTRO_TAMANHO_POR_DENTE = ToRadians(tooths_size / 2.0);
            double QUADRANTE_DENTE          = (2.0 * Math.PI / (double)number_tooths);

            // Raio
            double depth_center = ((double)pen.Width / 2.0);
            double rx = radius - depth_center;
            double ry = radius - depth_center;
            double inner_radius_x = rx * (1.0 - tooth_depth);
            double inner_radius_y = ry * (1.0 - tooth_depth);

            // Passo
            double RAD_PASSO = (2.0 * Math.PI / resolution);
            
            // Prepara Quadrantes
            List<QuadranteDenteEngrenagem> quadrantese = new List<QuadranteDenteEngrenagem>();
            double ix_quadrante = 0;
            double step = 0;
            int it = 0;
            while(it < number_tooths)
            {
                QuadranteDenteEngrenagem q = new QuadranteDenteEngrenagem()
                {
                    Index   = it,
                    Inicio  = ix_quadrante - CENTRO_TAMANHO_POR_DENTE,
                    Fim     = ix_quadrante + CENTRO_TAMANHO_POR_DENTE,
                    Interno = false
                };

                step = q.Inicio;
                while(step < q.Fim)
                {
                    float x = (float)(center_x + rx * Math.Cos(step));
                    float y = (float)(center_y + ry * Math.Sin(step));
                    q.Points.Add(new PointF(x,y));
                    step += RAD_PASSO;
                }

                quadrantese.Add(q);
                ix_quadrante += QUADRANTE_DENTE;
                it++;
            }

            List<QuadranteDenteEngrenagem> quadrantesi = new List<QuadranteDenteEngrenagem>();
            ix_quadrante = (QUADRANTE_DENTE / 2.0);
            it = 0;
            while (it < number_tooths)
            {
                QuadranteDenteEngrenagem q = new QuadranteDenteEngrenagem()
                {
                    Index   = it,
                    Inicio  = ix_quadrante - CENTRO_BASE_POR_DENTE,
                    Fim     = ix_quadrante + CENTRO_BASE_POR_DENTE,
                    Interno = false
                };

                step = q.Inicio;
                while (step < q.Fim)
                {
                    float x = (float)(center_x + inner_radius_x * Math.Cos(step));
                    float y = (float)(center_y + inner_radius_y * Math.Sin(step));
                    q.Points.Add(new PointF(x, y));
                    step += RAD_PASSO;
                }

                quadrantesi.Add(q);
                ix_quadrante += QUADRANTE_DENTE;
                it++;
            }

            List<PointF> points = new List<PointF>();
            int iq = 0;
            while(iq < quadrantese.Count)
            {
                points.AddRange(quadrantese[iq].Points);
                points.AddRange(quadrantesi[iq].Points);
                iq++;
            }
            gfx.DrawPolygon(pen, points.ToArray());
        }



        private static void DrawCircle(Graphics gfx, Pen pen, double radius, double center_x, double center_y, double num_theta)
        {
            int depth_center = (int)Math.Round((double)pen.Width / 2.0);

            double rx = radius - depth_center;
            double ry = radius - depth_center;

            List<PointF> points = new List<PointF>();
            float dtheta = (float)(2 * Math.PI / num_theta);
            float theta = 0;
            for (int i = 0; i < num_theta; i++)
            {
                float x = (float)(center_x + rx * Math.Cos(theta));
                float y = (float)(center_y + ry * Math.Sin(theta));
                points.Add(new PointF(x, y));
                theta += dtheta;
            }
            gfx.DrawPolygon(pen, points.ToArray());
        }

        private static double ToRadians(double angle)
        {
            return (Math.PI / 180.0) * angle;
        }
        private static double ToDegree(double radian)
        {
            return (180.0 / Math.PI) * radian;
        }

       

    }

    internal class QuadranteDenteEngrenagem
    {
        public double Inicio { get; set; }
        public double Fim { get; set; }
        public bool Interno { get; set; }
        public int Index { get; set; }
        public List<PointF> Points { get; set; }

        public QuadranteDenteEngrenagem()
        {
            Points = new List<PointF>();
        }
    }
}
