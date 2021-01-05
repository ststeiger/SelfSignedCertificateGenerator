
namespace TestApplicationHttps
{


    public class PseudoUrl
    {
        public string RawUrl;

        public string Scheme;
        public string Host;
        public int Port;

        public string Hash;
        public string QueryString;
        public string Path;


        public string Authority
        {
            get
            {
                return Host + ":" + Port.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }


        public PseudoUrl()
        { }


        public PseudoUrl(string value)
        {
            Parse(this, value);
        }


        public static PseudoUrl Parse(string url)
        {
            PseudoUrl pseudoUrl = new PseudoUrl();
            return Parse(pseudoUrl, url);
        }


        public static implicit operator PseudoUrl(string url)
        {
            return Parse(url);
        }


        public static PseudoUrl Parse(PseudoUrl pseudoUrl, string url)
        {
            //url = "https://foobar.com:8080/hello/kitty.aspx?noob=ish#foobar2000";
            //url = "https://*.com:8080/hello/kitty.aspx?noob=ish#foobar2000";
            //url = "foobar.com:8080/hello/kitty.aspx?noob=ish#foobar2000";
            //url = "foobar.com:8080/hello/kitty.aspx?noob=ish";
            //url = "foobar.com:8080/hello/kitty.aspx?#foobar2000";
            //url = "foobar.com:8080/hello/kitty.aspx";
            //url = "foobar.com:8080/";
            //url = "foobar.com:8080";
            //url = "foobar.com";
            //url = "*:8080";
            //url = "*";

            if (url == null)
                return null;

            pseudoUrl.RawUrl = url;

            int pos = url.IndexOf("://");

            if (pos != -1)
            {
                pseudoUrl.Scheme = url.Substring(0, pos);
                pseudoUrl.Scheme = pseudoUrl.Scheme.ToLowerInvariant();
                url = url.Substring(pos + 3);
            }
            else
                pseudoUrl.Scheme = "http";


            pos = url.IndexOf("#");
            if (pos != -1)
            {
                pseudoUrl.Hash = url.Substring(pos + 1);
                url = url.Substring(0, pos);
            }


            pos = url.IndexOf("?");
            if (pos != -1)
            {
                pseudoUrl.QueryString = url.Substring(pos + 1);
                url = url.Substring(0, pos);
            }

            pos = url.IndexOf("/");
            if (pos != -1)
            {
                pseudoUrl.Path = url.Substring(pos);
                url = url.Substring(0, pos);
            }


            pos = url.IndexOf(":");

            if (pos == -1)
            {
                pseudoUrl.Port = 80;
                pseudoUrl.Host = url;
            }
            else
            {
                pseudoUrl.Host = url.Substring(0, pos);
                url = url.Substring(pos + 1);
                pseudoUrl.Port = int.Parse(url, System.Globalization.CultureInfo.InvariantCulture);
            }

            return pseudoUrl;
        }


    } // End Class PseudoUrl 


}
