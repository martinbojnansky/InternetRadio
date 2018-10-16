using Helpers.Storage;
using InternetRadioShared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InternetRadioShared.Helpers
{
    public class InternetRadioHelper
    {
        /// <summary>
        /// Property gets and sets Json value of radio stations to local storage.
        /// </summary>
        private string RadiosJson
        {
            get { try { return LocalSettingsHelper.GetValue("Radios") as string; } catch { return null; } }
            set { LocalSettingsHelper.SetValue("Radios", value); }
        }

        /// <summary>
        /// All radio stations.
        /// </summary>
        public List<Radio> Radios { get; set; }
        /// <summary>
        /// Currently played radio station.
        /// </summary>
        public Radio CurrentRadio { get; set; }

        public InternetRadioHelper()
        {
            // Parse radio stations from the local storage.
            // Get static radio stations if they don't exists.
            if (RadiosJson != null)
                Radios = JsonHelper.FromJson<List<Radio>>(RadiosJson);
            else
                AddStaticRadios();
        }

        /// <summary>
        /// Creates static radios.
        /// </summary>
        /// <returns></returns>
        private void AddStaticRadios()
        {
            Radios = new List<Radio>();

            Radios.Add(new Radio("Funrádio", "http://stream.funradio.sk:8000/fun128.mp3"));            
            Radios.Add(new Radio("Európa 2", "http://ice2.europa2.sk/fm-europa2sk-128"));           
            Radios.Add(new Radio("Rádio Expres", "http://85.248.7.162:8000/96.mp3"));           
            Radios.Add(new Radio("Rádio FM", "http://live.slovakradio.sk:8000/FM_128.mp3"));            
            Radios.Add(new Radio("Rádio Jemné", "http://stream.jemne.sk/jemne-hi.mp3"));            
            Radios.Add(new Radio("Rádio Slovensko", "http://live.slovakradio.sk:8000/Slovensko_128.mp3"));            
            Radios.Add(new Radio("Clubradio", "http://icecast2.play.cz:8000/Clubradio.mp3"));            
            Radios.Add(new Radio("Rádio Impuls", "http://icecast5.play.cz/impuls128.mp3"));            
            Radios.Add(new Radio("OE3 Hitradio", "http://194.232.200.156:8000"));           
            Radios.Add(new Radio("Frekvence 1", "http://pool.cdn.lagardere.cz/fm-frekvence1-128"));           
            Radios.Add(new Radio("Fajn Rádio", "http://ice.abradio.cz:8000/fajn128.mp3"));            
            Radios.Add(new Radio("Kiss Publikum", "http://icecast8.play.cz/kisspublikum128.mp3"));           
            Radios.Add(new Radio("Best FM", "http://stream.bestfm.sk/128.mp3"));           
            Radios.Add(new Radio("BBC Radio 1", "http://bbcmedia.ic.llnwd.net/stream/bbcmedia_radio1_mf_q?s=1453540823&e=1453555223&h=2f79a47224002936f000e59235ca26dc"));           
            Radios.Add(new Radio("Capital XTRA London", "http://media-ice.musicradio.com/CapitalXTRALondonMP3"));            
            Radios.Add(new Radio("Evropa 2", "http://pool.cdn.lagardere.cz/fm-evropa2-128"));
            Radios.Add(new Radio("Funrádio SK/CZ", "http://stream.funradio.sk:8000/cs128.mp3"));

            SaveRadios();
        }
        
        /// <summary>
        /// Saves all radio stations to local storage.
        /// </summary>
        private void SaveRadios()
        {
            try
            {
                Radios = Radios.OrderBy(r => r.Title).ToList();
                RadiosJson = JsonHelper.ToJson(Radios);
            }
            catch { }
        }

        /// <summary>
        /// Gets all radio stations.
        /// </summary>
        /// <returns></returns>
        public SocketResponse GetRadios()
        {
            try
            {
                return new SocketResponse(SocketResponse.StatusCode.OK, RadiosJson);
            }
            catch (Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());
            }
        }

        /// <summary>
        /// Gets radio station from id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Radio GetRadio(string id)
        {
            return Radios.Find(r => r.Id == id);
        }

        /// <summary>
        /// Adds radio station.
        /// </summary>
        /// <param name="radio"></param>
        /// <returns></returns>
        public SocketResponse AddRadio(Radio radio)
        {
            try
            {
                Radios.Add(radio);
                SaveRadios();
                return new SocketResponse(SocketResponse.StatusCode.OK);
            }
            catch(Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());  
            }
        }

        /// <summary>
        /// Deletes radio station.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SocketResponse DeleteRadio(string id)
        {
            try
            {
                Radio radio = GetRadio(id);
                Radios.Remove(radio);
                SaveRadios();
                return new SocketResponse(SocketResponse.StatusCode.OK);
            }
            catch(Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());
            }
        }
    }
}
