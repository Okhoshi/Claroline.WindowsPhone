using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineApp.RT.Models
{
    public class ResourceModel<T> : ViewModelBase
    {
        public event SynchronizedHandler Synchronized;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedModel" /> class.
        /// </summary>
        public ResourceModel()
        {
            this.Items = new ObservableCollection<T>();
            this.TopItems = new ObservableCollection<T>();
            this.IsSynchronizing = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronizing
        {
            get { return GetProperty<bool>("IsSynchronizing"); }
            set { SetProperty<bool>("IsSynchronizing", value); }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public string ID
        {
            get { return GetProperty<string>("ID"); }
            set { SetProperty<string>("ID", value); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return GetProperty<string>("Name"); }
            set { SetProperty<string>("Name", value); }
        }

        public bool resNotif
        {
            get { return GetProperty<bool>("resNotif"); }
            set { SetProperty<bool>("resNotif", value); }
        }

        public bool isRes
        {
            get { return GetProperty<bool>("isRes"); }
            set { SetProperty<bool>("isRes", value); }
        }


        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<T> Items
        {
            get { return GetProperty<ObservableCollection<T>>("Items"); }
            set { SetProperty<ObservableCollection<T>>("Items", value); }
        }

        public ObservableCollection<T> TopItems
        {
            get { return GetProperty<ObservableCollection<T>>("TopItems"); }
            set { SetProperty<ObservableCollection<T>>("TopItems", value); }
        }

        /// <summary>
        /// Synchronizes the async.
        /// </summary>
        /*public async void SynchronizeAsync()
        {
            try
            {
                //
                // set synchronizing
                //
                IsSynchronizing = true;
                //
                // create client
                //
                HttpClient client = new HttpClient { MaxResponseContentBufferSize = 196608 };
                client.Timeout = new TimeSpan(0, 0, 120);
                //
                // get response asynchronously
                //
                HttpResponseMessage response = await client.GetAsync(Uri);
                //
                // get 
                //
                string payload = await response.Content.ReadAsStringAsync();
                //
                // create string reader
                //
                using (StringReader reader = new StringReader(payload))
                {
                    //
                    // create xml reader
                    //
                    using (XmlReader xml = XmlReader.Create(reader))
                    {
                        //
                        // clear items
                        //
                        AllItems.Clear();
                        //
                        // load data
                        //
                        XElement data = XElement.Load(xml);
                        //
                        // get items
                        //
                        var items = from x in data.Descendants("item") select x;
                        //
                        // loop over items
                        //
                        

                        foreach (var item in items)
                        {
                            var imageUrl = "";
                            XNamespace atom = "http://www.w3.org/2005/Atom";
                            XNamespace yahoo = "http://search.yahoo.com/mrss";
                            try
                            {
                            var desc = item.Element("description");
                            var descSplitted = desc.ToString().Replace("<![CDATA[", "");
                            descSplitted = descSplitted.Replace("]]>", "");
                            var cleandesc = descSplitted.ToString().Trim();
                            var cleanDescSplitted = cleandesc.Split('\"');

                         

                           
                                imageUrl = Utils.FindImageInXMLString(cleanDescSplitted, ".jpg");
                                
                            }
                            catch (Exception e)
                            {
                                imageUrl = "ms-appx:///assets/PlaceHolder.png";
                            }

                            try
                            {
                                AllItems.Add(new ItemModel
                                {
                                    ID = Utils.ParseElementValue(item, "title"),
                                    Title = WebUtility.HtmlDecode(Utils.ParseElementValue(item, "title")).Trim(),
                                    RawUri = Utils.ParseElementValue(item, "link"),
                                    Category = Utils.ParseElementValue(item, "category"),
                                    PublishedDate = DateTime.Parse(Utils.ParseElementValue(item, "pubDate")),
                                    PublishedWhen = DateTime.Parse(Utils.ParseElementValue(item, "pubDate")).ToString("F", new System.Globalization.CultureInfo("nl-BE")),
                                    Author = WebUtility.HtmlDecode(Utils.ParseElementValue(item, "author")).Trim(),
                                    RawDescription = (WebUtility.HtmlDecode(Utils.ParseElementValue(item, "description"))).Trim(),
                                    RawContent = (WebUtility.HtmlDecode(Utils.ParseElementValue(item, "description"))).Trim(),
                                    //ImageUri = Utils.ParseLinkValue(item, atom + "link", "href", "rel", "enclosure", "ms-appx:///assets/placeholder.png"),
                                    ImageUri = imageUrl,
                                    //This is a test to see if the whole image thing properly works and loads faster
                                    //ImageUri = Utils.ParseLinkValue(item,atom + "link" , imageUrl , "rel", "enclosure","ms-appx:///assets/placeholder.png"),
                                    Credit = WebUtility.HtmlDecode(Utils.ParseElementValue(item, yahoo + "description")).Trim(),
                                    //Credit = WebUtility.HtmlDecode(Utils.ParseElementValue(item, "description")).Trim(),
                                    Feed = this
                                });
                            }
                            catch
                            {
                            }
                        }
                        //
                        // done update
                        //
                        Resources.Clear();
                        //
                        // just sync
                        //
                        for (var i = 0; i < Math.Min(AllItems.Count, ApplicationModel.Current.MaxItemsToShow); i++)
                        {
                            //
                            // just add
                            //
                            Resources.Add(AllItems[i]);
                        }
                        //
                        // notify
                        //
                        ApplicationModel.Current.UpdateNotifications(this);
                        //
                        // all done
                        //
                        IsSynchronizing = false;
                        //
                        // notify
                        //
                        OnSynchronized(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception)
            {
                //
                // loading done
                //
                IsSynchronizing = false;
                //
                // notify
                //
                OnSynchronized(this, EventArgs.Empty);
            }
        }*/

        protected override void OnPropertyChanged(string name)
        {
            //
            // call base
            //
            base.OnPropertyChanged(name);
            //
            // check
            //
            switch (name)
            {
                case "MaxItemsToShow":
                    //
                    // resync
                    //
                    //SynchronizeAsync();
                    break;

            }
        }

        protected virtual void OnSynchronized(object sender, EventArgs e)
        {
            if (Synchronized != null)
            {
                Synchronized(sender, e);
            }
            //
            // internal hack for now should use delegate
            //
            //ApplicationModel.Current.NotifyFeedSynchronized(this);
        }
    }
}
