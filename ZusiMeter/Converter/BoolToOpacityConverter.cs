/** \file BoolToOpacityConverter.cs
 * \brief 'bool' zu 'Opacity' Converter
 */

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZusiMeter.Converter
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /** \class BoolToOpacityConverter
     * \brief 'bool' zu 'Opacity' Converter
     */
    [ValueConversion(typeof(bool), typeof(double))]
    public class BoolToOpacityConverter : IValueConverter
    {
        //-------------------------------------------------------------------------
        /** Konvertierung bool zu Opcaity
         * 
         * Wenn value = false hängt der Rückgabewert davon ab, ob parameter übergeben wurde.
         * Wurde Parameter übergegeben und ist parameter vom Typ double, wird parameter
         * zurückgegeben, anderenfalls 1.0.
         * @param value Steuerwert
         * @param parameter (optional) Rückgabewert bei value = false
         * @return value=true: 1.0
         */
        public static double Convert(bool value, object parameter)
        {
            return value ? 1.0 : ((parameter is double p) ? p :0.0);
        }

        //-------------------------------------------------------------------------
        /** Konvertierung Visibility zu bool
         * 
         * Wenn value oder parameter nicht vom Typ double sind wird false zurückgegeben.
         * Ebenfalls false wird zurückgegeben, wenn parameter übergeben wurde und mit value
         * übereinstimmt oder parameter nicht übergeben wurde und value != 1.0
         * ist. Bei value = 1.0 wird true zurückgegeben.
         * @param value Steuerwert
         * @param parameter (optional) siehe Beschreibung
         * @return value=1.0: true
         */
        public static bool ConvertBack(double value, object parameter)
        {
            if (value == 1.0)
            {
                return true;
            }

            if (parameter is double p)
            {
                return value != p;
            }

            return false;
        }

        //-------------------------------------------------------------------------
        /** Konvertierung bool zu Opacity
         * 
         * Wenn value = false hängt der Rückgabewert davon ab, ob parameter übergeben wurde.
         * Wurde parameter übergegeben und ist parameter vom Typ double, wird parameter
         * zurückgegeben, anderenfalls 0.0.
         * @param value Steuerwert
         * @param targetType n.v.
         * @param parameter (optional) Rückgabewert bei value = false
         * @param culture n.v.
         * @return value=true: 1.0
         */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                bool b => Convert(b, parameter),
                _ => 0.0,
            };
        }

        //-------------------------------------------------------------------------
        /** Konvertierung Opacity zu bool
         * 
         * Wenn value oder parameter nicht vom Typ double sind wird false zurückgegeben.
         * Ebenfalls false wird zurückgegeben, wenn parameter übergeben wurde und mit value
         * übereinstimmt oder parameter nicht übergeben wurde und value != 1.0
         * ist. Bei value = 1.0 wird true zurückgegeben.
         * @param value Steuerwert
         * @param targetType n.v.
         * @param (optional) siehe Beschreibung
         * @param culture n.v.
         * @return value=1.0: true
         */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                double v => ConvertBack(v, parameter),
                _ => false,
            };
        }
    }

    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    [ValueConversion(typeof(bool), typeof(Visibility))]
    /** \class ReverseBoolToVisibilityConverter
     * \brief Umgekehrter 'bool' zu 'Visibility' Converter
     */
    public class ReverseBoolToOpacityConverter : IValueConverter
    {
        //-------------------------------------------------------------------------
        /** Konvertierung bool zu Visibility
         * 
         * Wenn value = true hängt der Rückgabewert davon ab, ob parameter übergeben wurde.
         * Wurde parameter übergegeben und ist parameter vom Typ double, wird parameter
         * zurückgegeben, anderenfalls 0.0.
         * @param value Steuerwert
         * @param targetType n.v.
         * @param parameter (optional) Rückgabewrt bei value = false
         * @param culture n.v.
         * @return value=false: 1.0
         */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                bool b => BoolToOpacityConverter.Convert(!b, parameter),
                _ => 0.0,
            };
        }

        //-------------------------------------------------------------------------
        /** Konvertierung Opacity zu bool
         * 
         * Wenn value oder parameter nicht vom Typ double sind wird true zurückgegeben.
         * Ebenfalls true wird zurückgegeben, wenn parameter übergeben wurde und mit value
         * übereinstimmt oder parameter nicht übergeben wurde und value != 1.0
         * ist. Bei value = 1.0 wird false zurückgegeben.
         * @param value Steuerwert
         * @param targetType n.v.
         * @param (optional) siehe Beschreibung
         * @param culture n.v.
         * @return value=1.0: false
         */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                double v => !BoolToOpacityConverter.ConvertBack(v, parameter),
                _ => false,
            };
        }
    }
}
