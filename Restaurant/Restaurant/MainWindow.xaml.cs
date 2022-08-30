using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restaurant
{
    
     /// <summary>
      /// Interaction logic for MainWindow.xaml
      /// </summary>
    public partial class MainWindow : Window
    {

        Restaurant our_resto = new Restaurant();

        public Restaurant our_restoProp
        {
            get { return our_resto; }
            set { our_resto = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            stock_resto.ItemsSource = our_resto.stock_restoProp.list_stock;
            Tresorerie.DataContext = our_resto;
            name_resto.Text = our_resto.NameProp;
        }

        private void command_table(object sender, RoutedEventArgs e)
        {
            Command pageCommand = new Command();
            //this.Visibility = Visibility.Hidden;
            pageCommand.Show();
        }

        private void add_new_ingredient(object sender, RoutedEventArgs e)
        {
            AddIng deuxiemePage = new AddIng();
            //this.Visibility = Visibility.Hidden;
            deuxiemePage.Show();
        }

        private void add_new_recipe(object sender, RoutedEventArgs e)
        {
            AddRecipe ajoutRecette = new AddRecipe();
            //this.Visibility = Visibility.Hidden;
            ajoutRecette.Show();
        }

        private void click_view_menu(object sender, RoutedEventArgs e)
        {
            viewMenu listeMenu = new viewMenu();
            listeMenu.Show();
        }

        private void modify_menu(object sender, RoutedEventArgs e)
        {

        }

    }

    public class Recipe
    {
        public string Name { get; set; }
        public ObservableCollection<Ingredient> list_ing;
        public double Price { get; set; }

        public Recipe(string name, double price)
        {
            this.Name = name;
            this.Price = price;
            list_ing = new ObservableCollection<Ingredient>();
        }

        public Recipe(string name, double price, ObservableCollection<Ingredient> list)
        {
            this.Name = name;
            this.Price = price;
            list_ing = list;
        }

        public void add_ingredient_to_recipe(Ingredient new_ing)
        {
            list_ing.Add(new_ing);
        }

        public ObservableCollection<Ingredient> passer_commande()
        {
            return list_ing;
        }
    }


    public class Ingredient : INotifyPropertyChanged
    {
        private string _name;
        private double _quantity;


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(this, "Name");
            }
        }

        public double Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                OnPropertyChanged(this, "Quantity");
            }
        }

        public Ingredient(string name, double quantity)
        {
            this.Name = name;
            this.Quantity = quantity;
        }

        public bool add_ingredient(double quantity)
        {
            bool response = true;
            if (quantity < 0 && this.Quantity < Math.Abs(quantity))
            {
                response = false;
            }
            else
            {
                this.Quantity -= quantity;
            }


            return response;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(object sender, string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(Property));
            }
        }
    }

    public class Stock
    {
        public ObservableCollection<Ingredient> list_stock;

        public Stock()
        {
            list_stock = new ObservableCollection<Ingredient>();
        }

        public void modify_stock(string name_ing, double quant_ing)
        {
            bool found = false;
            foreach (Ingredient ing in list_stock)
            {
                if (ing.Name == name_ing)
                {
                    found = true;
                    ing.add_ingredient(quant_ing);

                }
            }
            if (!found)
            {
                Ingredient new_ing = new Ingredient(name_ing, quant_ing);
                list_stock.Add(new_ing);
            }
        }


        public bool add_new_ing(string name_ing, double quant_ing)
        {
            bool not_found = true;
            foreach (Ingredient ing in list_stock)
            {
                if (ing.Name == name_ing)
                {
                    not_found = false;


                }
            }
            if (not_found)
            {
                Ingredient new_ing = new Ingredient(name_ing, quant_ing);
                list_stock.Add(new_ing);

            }
            return not_found;
        }

        public bool use_a_recipe(Recipe asked_recipe)
        {   // we get all the ingredient from the recipe
            ObservableCollection<Ingredient> list_to_use = asked_recipe.passer_commande();

            // this part will find if the ingredient is in stock
            bool possible = true;

            for (int ing = 0; ing < list_to_use.Count; ing++)
            {
                bool is_present = false;
                for (int ing_sto = 0; ing_sto < list_stock.Count; ing_sto++)
                {
                    if (list_to_use[ing].Name == list_stock[ing_sto].Name && list_to_use[ing].Quantity <= list_stock[ing].Quantity)
                    {
                        is_present = true;
                    }
                }

                if (possible)
                {
                    possible = is_present;
                }


            }

            if (possible)
            {
                for (int ing = 0; ing < list_to_use.Count; ing++)
                {

                    for (int ing_sto = 0; ing_sto < list_stock.Count; ing_sto++)
                    {
                        if (list_to_use[ing].Name == list_stock[ing_sto].Name)
                        {
                            list_stock[ing_sto].add_ingredient(list_to_use[ing].Quantity);
                        }
                    }

                }

            }
            else
            {
                // messagebox : not enough ingredient
            }

            return possible;
        }
    }



    public class Restaurant : INotifyPropertyChanged
    {
        // nom du restaurant
        string Name { get; set; }

        public string NameProp
        {
            get { return Name; }
            set { Name = value; }
        }

        // fond monétaire du restaurant
        public double _tresorerie;


        public double Tresorerie
        {
            get
            {
                return _tresorerie;
            }
            set
            {
                _tresorerie = value;
                OnPropertyChanged(this, "Tresorerie");
            }
        }

        // stock d'ingredient
        Stock stock_resto;
        public Stock stock_restoProp
        {
            get { return stock_resto; }
            set { stock_resto = value; }
        }

        // liste des recettes/plats à proposer 
        ObservableCollection<Recipe> list_plat;
        public ObservableCollection<Recipe> list_platProp
        {
            get { return list_plat; }
            set { list_plat = value; }
        }


        // constructor
        public Restaurant()
        {
            stock_resto = new Stock();
            list_plat = new ObservableCollection<Recipe>();

            StreamReader setup = new StreamReader("setup_resto.txt");
            StreamReader recipe = new StreamReader("recette.txt");


            // this part set up the name of the restaurant and its stock of ingredient.
            string line_1 = "";
            line_1 = setup.ReadLine();
            this.Name = line_1;

            line_1 = setup.ReadLine();
            line_1 = setup.ReadLine();

            this.Tresorerie = Convert.ToDouble(line_1);
            line_1 = setup.ReadLine();
            while (setup.Peek() > 0)
            {
                line_1 = setup.ReadLine();
                string[] data_ing = line_1.Split(';');

                stock_resto.add_new_ing(data_ing[0], Convert.ToDouble(data_ing[1]));

            }

            setup.Close();

            // this part create all the recipe for the restaurant.

            string line_2 = "";

            while (recipe.Peek() > 0)
            {
                line_2 = recipe.ReadLine();
                //MessageBox.Show(line_2);
                string[] name_price = line_2.Split('-');
                Recipe new_recipe = new Recipe(name_price[0], Convert.ToDouble(name_price[1]));
                line_2 = recipe.ReadLine();
                while (line_2 != "---")
                {

                    string[] data_line2 = line_2.Split(';');
                    //MessageBox.Show(data_line2[0] + " " + data_line2[1]);
                    Ingredient new_ing = new Ingredient(data_line2[0], Convert.ToDouble(data_line2[1].Trim('\r')));
                    new_recipe.add_ingredient_to_recipe(new_ing);
                    line_2 = recipe.ReadLine();
                }

                list_plat.Add(new_recipe);
            }

            recipe.Close();
        }


        // return the list of all recipe - to be use when asked for the Menu
        public ObservableCollection<Recipe> display_plat()
        {
            foreach (Recipe rec in list_plat)
            {
                Console.WriteLine(rec.Name + " - " + Convert.ToString(rec.Price) + "€");
            }
            return list_plat;
        }

        // After a window, take a string and a double to create a new ingredient if it did not exist before (return bool) to the stock
        public bool add_new_ingredient(string name, double quant)
        {
            bool possible = true;

            possible = this.stock_resto.add_new_ing(name, quant);
            return possible;
        }

        // after a window asking for a string[] (to get the way we want), add a recipe to the list_plat.
        public void add_new_recipe(string[] new_recipe)
        {
            // take a string[] witht the first case being the name of the recipe, and the other cases are the ingredient name with their quantity
            // exemple : [burger-12.0,farine;0.2,tomate;0.1,bacon;0.3]
            if (new_recipe.Length > 0)
            {
                string[] name_price = new_recipe[0].Split('-');
                Recipe new_rec = new Recipe(name_price[0], Convert.ToDouble(name_price[1]));


                for (int i = 1; i < new_recipe.Length; i++)
                {
                    string[] data_rec = new_recipe[i].Split(';');

                    Ingredient new_ing = new Ingredient(data_rec[0], Convert.ToDouble(data_rec[1]));

                    new_rec.add_ingredient_to_recipe(new_ing);
                }
                this.list_plat.Add(new_rec);
            }
        }


        public bool take_a_command(string name_recipe)
        {
            bool possible = false;

            foreach (Recipe rec_in_sto in list_plat)
            {
                if (rec_in_sto.Name == name_recipe)
                {
                    Console.WriteLine("test if ing");
                    possible = this.stock_resto.use_a_recipe(rec_in_sto);
                }
                if (possible)
                {
                    Tresorerie = Tresorerie + rec_in_sto.Price;
                    Console.WriteLine(Tresorerie);
                    break;
                }

            }

            return possible;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(object sender, string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(Property));
            }
        }
    }
}

