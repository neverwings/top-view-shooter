using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class GameList
    {
        private static List<Game> _gameList;
        public static List<Game> gameList
        {
            get { return _gameList; }
            set { _gameList = value; }
        }

        public GameList()
        {
            gameList = new List<Game>();
        }

        public static String addGame(Game gameObj)
        {
            
            gameList.Add(gameObj);
            return gameObj.getName();
        }

        public static String removeGame(String name)
	    {
		    foreach(Game game in gameList)
		    {
			    if(game.getName() == name)
			    {
				    gameList.Remove(game);
                    return name;
			    }
		    }
            return null;
	    }

        public static Game getGame(String username)
	    {
		    foreach(Game game in gameList)
		    {
			    if(game.getName() == username)
			    {
				    return game;
			    }
		    }
		    return null;
	    }

        public static List<Game> getList()
        {
            return gameList;
        }
    }

    class Game
    { 
        private String gameName;
        private String gameIP;

        public Game(String name, String IP)
        {
            this.gameIP = IP;
            this.gameName = name;
        }

	    public String getName()
	    {
		    return this.gameName;
	    }
	
	    public String getIP()
	    {
		    return this.gameIP;
	    }
    }
}
