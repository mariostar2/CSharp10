using System;
using System.Collections.Generic;

namespace CSharp10
{
    internal class Program
    {
        delegate void ThereIsFire(string location);
      
            
        static void Call119(string location)
        {
            Console.WriteLine($"소방서죠 {location} 에 불이났습니다");
        }

        static void ShotOut(string location)
        {
            Console.WriteLine($"{location}에 불이 났습니다");
        }

        static void Escape(string location)
        {
            Console.WriteLine($"{location}에서 탈출하자");
        }
    

    static void Main(string[] args)
        {
            if (true)
            {
                   
                GameManager gameManager = new GameManager();

                Player[] players = new Player[5];
                for (int i = 0; i < players.Length; i++)
                    players[i] = gameManager.GetNewPlayer();

                Console.WriteLine("모든 플레이어 에게 2000데미지 전달");
                foreach (Player p in players)
                    p.TakeDamage(2000);

                gameManager.ShowAllPlayer();
                gameManager.Notify("안녕하세요 GM중고나라 입니다.");
                gameManager.Notify("잠시후 17시부터 사이트 점검이 있을 예정입니다.");
            }

            //델리게이트 체인.
            //함수를 추가로 참조하는게 가능하다. 
            ThereIsFire onFire = null;
            onFire = Escape;
            onFire += ShotOut;
            onFire += Call119;

            onFire -= ShotOut;          //델리게이터에서 해상 함수를 제거한다.
            onFire("우리집");

        }     
    }

    //함수가 아니고 자료형의 선언이기 때문에 내용부가 없다.
    public delegate void PlayerEvent(Player player);
    public delegate void NotifyEvent(string str);
    
    public class GameManager
    {

        //모든 플레이어가 담겨있는 리스트(class)
        List<Player> playerList = new List<Player>();
        List<Player> deadPlayerList = new List<Player>();

        //모든 사용자에게 공지사항을 보내는 함수.(델리게이트)
        //=> 모든 사용자가 공지를 받는 함수를 담고 있다.
        NotifyEvent onNotifyAllPlayer = null;
        
        //게임메니저의 요청에 의해 새로운 플레이어를 만들어 외부에 전달함
        public Player GetNewPlayer()
        {
            Console.WriteLine("새로운 플레이어 생성");
            Console.Write("이름:");
            string name = Console.ReadLine();                     
            int hp = 0;
            while(hp <= 0)
            {
                try
                {
                    Console.Write("체력:");
                    hp = int.Parse(Console.ReadLine());
                }

                catch(Exception e)
                {
                    Console.WriteLine($"HP 입력에 실패하였습니다. ({e.Message})");
                    hp = 0;
                }
            }
           
            //입력받은 값을 통해 새로운 플레이어 객체를 생성한다
            Player newPlayer = new Player(name, hp, OnDeadPlayer);
            playerList.Add(newPlayer);

            onNotifyAllPlayer += newPlayer.OnNotfity;

            return newPlayer;
        }


        //현재 모든 플레이어의 정보를 출력
        public void ShowAllPlayer()
        {
            Console.WriteLine("모든 플레이어 정보");
            Console.WriteLine("[생존]-----------------");
            for(int i =0; i<playerList.Count; i++)
            {
               Player player = playerList[i];      //i번쨰 플레이어 대입
                Console.WriteLine(player);

            }
            Console.WriteLine("[죽음]-----------------");
            foreach(Player deadPlayer in deadPlayerList)
            {
                Console.WriteLine(deadPlayer);
            }
            Console.WriteLine("-------------------");
        }
        
        //공지사항을 전달하는 함수
        public void Notify(string notify)
        {
            onNotifyAllPlayer(notify);
        }
        // 누군가가 죽었을때 불리는 이벤트 함수
        private void OnDeadPlayer(Player player)
        {
            playerList.Remove(player);              //리스트 내부에서 player를 제거함
            deadPlayerList.Add(player);             //이후 죽은 플레이어 리스트에 추가
        }
    }
    public class Player
    {
        //const,readonly :변수를 상수화 시키는 키워드.
        //const :선언과 동시에 초기화를 해야한다
        //readonly : 생성자에 한하여 초기 값을 대입할 수 있다.
        public readonly int MAX_HP;            //최대 체력은 변하지 않기 때문에 상수로 만든다.
        const int MAX_NAME_LENGTH = 12; //이 값은 모든 player객체가 동일하기 때문에 const 상수를 사용한다.

        string name;
        int hp;
        PlayerEvent onDead;

        //프로퍼티 (속성),(읽기 전용)
        public string Name => name;     //get에 한해서 표현식을 생략 할수 있다.(=람다식)
        
        // bool 앞에는 is를붙임(규칙)
        public bool IsAlive => hp > 0;  //특정 수식을 통해 프로퍼티의 값을 꾸밀 수 있다
        
        public Player(string name, int hp, PlayerEvent onDead)
        
        {
            MAX_HP = hp; //readonly 상수는 객체별 다른 값을 할당할 수 있다.

            this.name = name;
            this.hp = hp;
            this.onDead = onDead;
        }

        
        public void TakeDamage(int power)
        {
            hp -= power;
            if (hp <= 0)
            {
                hp = 0;
                
                onDead(this);
            }
        }
        
        //GM이 보내는 공지사항을 player가 받는 함수
        public void OnNotfity(string notify)
        {
            Console.WriteLine($"{name}이 공지사항을 받았다 : {notify}");
        }

        
        

        //Player 객체를 출력할 떄 항상 아래의 규격에 따르게 만든다.
        //따라서 힘들게 hp 따로 maxHP따로 안만들어도된다
        public override string ToString()
        {
            return $"{name}({hp}/ {MAX_HP})";
        }

    }
}
