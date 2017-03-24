using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;
using System.Windows.Threading;
using System.Web;
//using System.Text.RegularExpressions;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public delegate void RequestQuery(String message);
        public RequestQuery requestQuery;

        private string privateMsgTo = "";
        private string username = "ANON";
        private List<String> players_List;

        public Window1(string user)
        {
            this.username = user;
            new GameList();
            this.players_List = new List<String>();
            InitializeComponent();
            //this.chatList.Items.Add("Test:test");
            //pList.Add("anon");
            //this.playerList.Items.Add("Anon");
            //this.addGame("game1", "0.0.0.0");
            gameList.MouseDoubleClick += gameList_MouseDoubleClick;
            Closing += Window1_Closing;

            requestQuery = new RequestQuery(this.addMessageDelegate);
            
            new HTTP(requestQuery);//this);
            this.Show();
            HTTP.sendToServer("type=player&subtype=all&username="+username);

        }

        public Window1(string user, string URL)
        {
            this.username = user;
            new GameList();
            this.players_List = new List<String>();
            InitializeComponent();
            //this.chatList.Items.Add("Test:test");
            //pList.Add("anon");
            //this.playerList.Items.Add("Anon");
            //this.addGame("game1", "0.0.0.0");
            gameList.MouseDoubleClick += gameList_MouseDoubleClick;
            Closing += Window1_Closing;

            requestQuery = new RequestQuery(this.addMessageDelegate);

            new HTTP(requestQuery, URL);//this);
            this.Show();
            HTTP.sendToServer("type=player&subtype=all&username=" + username);

        }

        public/*private*/ void addMessageDelegate(String message)
        {
            this.decipherURL(message);
        }

        public void decipherURL(String url)//, Window1 mT)
        {
            String data = url.Split('?')[1];
            String[] args = data.Split('&');
            String type = "";
            String aux1 = "";
            String aux2 = "";
            String aux3 = "";
            String aux4 = "";

            foreach (String arg in args)
            {
                String[] line = arg.Split('=');
                if (line[0].Equals("type"))
                {
                    type = line[1];
                }
                else
                {
                    if (type == "chat")
                    {
                        if (line[0] == "username")
                        {
                            aux1 = line[1];
                        }
                        if (line[0] == "message")
                        {
                            aux2 = line[1];
                        }
                        if (line[0] == "personal")
                        {
                            aux3 = line[1];
                        }
                    }
                    if (type == "game")
                    {
                        if (line[0] == "subtype")
                        {
                            aux1 = line[1];
                        }
                        if (line[0] == "name")
                        {
                            aux2 = line[1];
                        }
                        if(line[0] == "players")
                        {
                            aux3 = line[1];
                        }
                        if(line[0] == "ipaddress")
                        {
                            aux4 = line[1];
                        }
                    }
                    if (type == "player")
                    {
                        if (line[0] == "subtype")
                        {
                            aux1 = line[1];
                        }
                        if (line[0] == "name")
                        {
                            aux2 = line[1];
                        }
                        if (line[0] == "ammount")
                        {
                            aux3 = line[1];
                        }
                    }
                }
            }
            if (type == "chat")
            {
                string username = aux1;
                string message = aux2;
                message = message.Replace("_~~~_~_~~~_", " ");
                string personalFrom = aux3;
                if (personalFrom != "")
                    message = "(personal):: "+message;
                //send back to window thread
                chatList.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    chatList.Items.Add(username + ": " + message);
                }));
            }
            if (type == "game")
            {
                string subtype = aux1;
                string name = aux2;
                string ammountOfPlayers = aux3;
                string ipaddress = aux4;
                if(subtype == "add")
                {
                    if (GameList.getGame(name) != null)
                    {
                        GameList.addGame(new Game(name, ipaddress));
                    }
                    else {
                        //remove from gamelist
                        //foreach(String s i
                        chatList.Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            //chatList.Items.Add(username + ":" + personalFrom + message);
                        }));
                    }


                    gameList.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        gameList.Items.Add(name + "("+ ammountOfPlayers+ ")");
                    }));
                }
            }
            if (type == "player")
            {
                string subtype = aux1;
                string name = aux2;
                string ammount = aux3;
                playerList.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    if (subtype == "add")
                    {
                        players_List.Add(name);
                        playerList.Items.Add(name);
                    }
                    else if (subtype == "all")
                    {
                        Console.WriteLine("onecheck");
                        string[] values = name.Split('-');
                        playerList.Items.Clear();
                        foreach (string s in values)
                        {
                            playerList.Items.Add(s);
                            players_List.Add(s);
                        }
                    }
                    else if (subtype == "remove")
                    {
                        playerList.Items.Remove(name);
                        players_List.Remove(name);
                    }
                }));
            }
        }

        void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HTTP.sendToServer("type=player&subtype=remove&username=" + username);
            HTTP.chatThread.Abort();
            HTTP.closeConnection();
            //HTTP.chatThread.Suspend();
        }

        void gameList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        { 
            if (gameList.SelectedIndex == -1)
            {
                return;
            }
            String NAME = gameList.SelectedValue.ToString();
            String IP = GameList.getGame(NAME).getIP();
            openGame(NAME, IP);

            //throw new NotImplementedException();
        }

        private void MenuItemPrivateMessage_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(playerList.SelectedValue == username+"::"+ playerList.SelectedValue+ "::" + username);
            if (!playerList.SelectedValue.Equals(username))
            {
                /*String[] texts = chatBox.Text.Split(new string[] { "[personal]" }, StringSplitOptions.None);
                String userName = "";
                if (texts.Length > 1)
                {
                    for (int i = 2; i < texts.Length; i++)
                        userName += texts[i];
                }
                else
                {
                    userName = chatBox.Text;
                }
                chatBox.Text = playerList.SelectedValue + "[personal]" + userName;
                chatBox.Focusable = true;
                Keyboard.Focus(chatBox);
                chatBox.ScrollToEnd();
                chatBox.CaretIndex = chatBox.Text.Length;
                var rect = chatBox.GetRectFromCharacterIndex(chatBox.CaretIndex);
                chatBox.ScrollToHorizontalOffset(rect.Right);*/
                privateMsgTo = (string)playerList.SelectedValue;
                chatBoxGroup.Header = playerList.SelectedValue;
            }
            else
            {
                MessageBox.Show("Cannot Send Private Message To Self.");
            }

        }

        private void MenuItemDePrivatizeMessage_Click(object sender, RoutedEventArgs e)
        {
            /*String[] texts = chatBox.Text.Split(new string[] { "[personal]" }, StringSplitOptions.None);
            String userName = "";
            if (texts.Length > 1)
            {
                for (int i = 2; i < texts.Length; i++)
                    userName += texts[i];
            }
            else
            {
                userName = chatBox.Text;
            }
            chatBox.Text = userName;
            chatBox.Focusable = true;
            Keyboard.Focus(chatBox);
            chatBox.ScrollToEnd();
            chatBox.CaretIndex = chatBox.Text.Length;
            var rect = chatBox.GetRectFromCharacterIndex(chatBox.CaretIndex);
            chatBox.ScrollToHorizontalOffset(rect.Right);*/
            privateMsgTo = "";
            chatBoxGroup.Header = "All:";

        }


        private void MenuItemJoinGame_Click(object sender, RoutedEventArgs e)
        {
            if (gameList.SelectedIndex == -1)
            {
                return;
            }
            String NAME = gameList.SelectedValue.ToString();
            String IP = GameList.getGame(NAME).getIP();
            openGame(NAME, IP);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (chatBox.Text != "")
            {
                //TextBox chatTextBlock = (TextBox)this.FindName("chatBox");
                String message = chatBox.Text;
                String privateM = "";
                if (privateMsgTo != "")
                    privateM = "&personal=" + privateMsgTo;
                String urlParameters = "type=chat&username=" + this.username + "&message=" + message + privateM;
                Console.WriteLine(urlParameters);
                Console.WriteLine("type");
                HTTP.sendToServer(urlParameters);
                //ListBox chatListBox = (ListBox)this.FindName("chatList");
                //chatBox.Items.Add("user:" + message);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            HTTP.sendToServer("type=player&subtype=remove&username=" + username);
            HTTP.chatThread.Abort();
            HTTP.closeConnection();
            new Login();
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            HTTP.sendToServer("type=player&subtype=remove&username=" + username);
            HTTP.chatThread.Abort();
            HTTP.closeConnection();
            this.Close();
        }

        private void openGame(String gameNAME, String gameIPADDRESS)
        {
            //start game lobby 
            
        }

        private void addGame(String name, String IP)
        {
            this.gameList.Items.Add(GameList.addGame(new Game(name, IP)));
        }

        private void removeGame(String name)
        {
            String gname = GameList.removeGame(name);
            if (gname != "")
                this.gameList.Items.Remove(gname);
            else
            {
                MessageBox.Show("An error was found removing game with name \"" + gname + "\".");
            }
        }
    }
}