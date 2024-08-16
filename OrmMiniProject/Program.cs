using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using OrmMiniProject.Contexts;
using OrmMiniProject.DTOs.Order;
using OrmMiniProject.DTOs.OrderDetailDTO;
using OrmMiniProject.DTOs.Payment;
using OrmMiniProject.DTOs.Product;
using OrmMiniProject.DTOs.User;
using OrmMiniProject.Enums;
using OrmMiniProject.Exceptions;
using OrmMiniProject.Models;
using OrmMiniProject.Repositories.Implementations;
using OrmMiniProject.Repositories.Interfaces;
using OrmMiniProject.Services.Implementations;
using OrmMiniProject.Services.Interfaces;

namespace OrmMiniProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var dbContext = new AppDbContext();

            IProductRepository productRepository = new ProductRepository(dbContext);
            IUserRepository userRepository = new UserRepository(dbContext);
            IOrderRepository orderRepository = new OrderRepository(dbContext);
            IOrderDetailRepository orderDetailRepository = new OrderDetailRepository(dbContext);
            IPaymentRepository paymentRepository = new PaymentRepository(dbContext);

            IProductService productService = new ProductService(productRepository);
            IUserService userService = new UserService(userRepository, orderRepository);
            IOrderService orderService = new OrderService(orderRepository, orderDetailRepository, productRepository);
            IPaymentService paymentService = new PaymentService(paymentRepository, orderRepository, userRepository);

            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n=== Mini E-Commerce Console App ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Manage Products");
                Console.WriteLine("4. Manage Users");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                string mainChoice = Console.ReadLine();

                switch (mainChoice)
                {
                    case "1":
                        var loggedInUser = await LoginAsync(userService);
                        if (loggedInUser != null)
                        {
                            if (loggedInUser.Email == "bendiyevf@gmail.com")
                            {
                                await AdminMenuAsync(productService, userService);
                            }
                            else
                            {
                                await UserMenuAsync(loggedInUser, orderService, paymentService, productService, userService);
                            }
                        }
                        break;
                    case "2":
                        await RegisterUserAsync(userService);
                        break;
                    case "3":
                        if (await AdminLoginAsync(userService))
                        {
                            await ManageProductsAsync(productService);
                        }
                        break;
                    case "4":
                        if (await AdminLoginAsync(userService))
                        {
                            await ManageUsersAsync(userService);
                        }
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Exiting the application. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static async Task<UserDTO> LoginAsync(IUserService userService)
        {
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            try
            {
                var user = await userService.LoginAsync(email, password);
                if (user != null)
                {
                    Console.WriteLine($"Welcome, {user.FullName}!");
                    return user;
                }
                else
                {
                    Console.WriteLine("Invalid email or password.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        static async Task<bool> AdminLoginAsync(IUserService userService)
        {
            var adminUser = await LoginAsync(userService);
            return adminUser != null && adminUser.Email == "bendiyevf@gmail.com";
        }

        static async Task AdminMenuAsync(IProductService productService, IUserService userService)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- Admin Menu ---");
                Console.WriteLine("1. Manage Products");
                Console.WriteLine("2. Manage Users");
                Console.WriteLine("0. Logout");
                Console.Write("Select an option: ");
                string adminChoice = Console.ReadLine();

                switch (adminChoice)
                {
                    case "1":
                        await ManageProductsAsync(productService);
                        break;
                    case "2":
                        await ManageUsersAsync(userService);
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static async Task UserMenuAsync(UserDTO user, IOrderService orderService, IPaymentService paymentService, IProductService productService, IUserService userService)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- User Menu ---");
                Console.WriteLine("1. Manage Orders");
                Console.WriteLine("2. Manage Payments");
                Console.WriteLine("0. Logout");
                Console.Write("Select an option: ");
                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1":
                        await ManageOrdersAsync(orderService, paymentService, productService, user.Id);
                        break;
                    case "2":
                        await ManagePaymentsAsync(paymentService, orderService, userService,user.Id, user);
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        #region Order Management
        static async Task ManageOrdersAsync(
    IOrderService orderService,
    IPaymentService paymentService,
    IProductService productService,
    int userId)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- Order Management ---");
                Console.WriteLine("1. Create Order");
                Console.WriteLine("2. List My Orders");
                Console.WriteLine("3. Get Order Details");
                Console.WriteLine("4. Cancel Order");
                Console.WriteLine("0. Back to User Menu");
                Console.Write("Select an option: ");
                string orderChoice = Console.ReadLine();

                try
                {
                    switch (orderChoice)
                    {
                        case "1":
                            await CreateOrderAsync(orderService, userId, productService);
                            break;
                        case "2":
                            await ListUserOrdersAsync(orderService, userId);
                            break;
                        case "3":
                            await GetOrderDetailsByOrderIdAsync(orderService);
                            break;
                        case "4":
                            await CancelOrderAsync(orderService);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        static async Task CancelOrderAsync(IOrderService orderService)
        {
            Console.WriteLine("\n--- Cancel Order ---");
            Console.Write("Enter Order ID to Cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            await orderService.CancelOrderAsync(orderId);
            Console.WriteLine("Order canceled successfully!");
        }

        static async Task CreateOrderAsync(IOrderService orderService, int userId, IProductService productService)
        {
            Console.WriteLine("\n--- Create New Order ---");
            var selectedProducts = new List<CreateOrderDetailDTO>();
            bool addMoreProducts = true;

            while (addMoreProducts)
            {
                Console.WriteLine("Available Products:");
                var products = await ListAllProductsAsync(productService);

                if (!products.Any())
                {
                    Console.WriteLine("No products available to order.");
                    return;
                }

                Console.Write("Enter Product ID to Add to Order: ");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Invalid ID format.");
                    return;
                }

                Console.Write("Enter Quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Invalid quantity format.");
                    return;
                }

                var product = await productService.GetProductByIdAsync(productId);
                if (quantity > product.Stock)
                {
                    Console.WriteLine($"Insufficient stock for {product.Name}. Available stock: {product.Stock}. Order canceled.");
                    return;
                }

                selectedProducts.Add(new CreateOrderDetailDTO
                {
                    ProductId = productId,
                    Quantity = quantity
                });

                Console.Write("Do you want to add another product? (y/n): ");
                addMoreProducts = Console.ReadLine()?.ToLower() == "y";
            }

            decimal totalAmount = 0;
            foreach (var orderDetail in selectedProducts)
            {
                var product = await productService.GetProductByIdAsync(orderDetail.ProductId);
                totalAmount += product.Price * orderDetail.Quantity;
            }

            Console.WriteLine("\n--- Invoice ---");
            foreach (var orderDetail in selectedProducts)
            {
                var product = await productService.GetProductByIdAsync(orderDetail.ProductId);
                Console.WriteLine($"Product: {product.Name} | Quantity: {orderDetail.Quantity} | Price Per Item: {product.Price:C} | Subtotal: {product.Price * orderDetail.Quantity:C}");
            }
            Console.WriteLine($"Total Amount: {totalAmount:C}");

            Console.Write("Do you want to proceed with the order? (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("Order creation canceled.");
                return;
            }

            var newOrder = new CreateOrderDTO
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending
            };

            await orderService.CreateOrderAsync(newOrder);

            var createdOrder = await orderService.GetOrdersAsync();
            var orderId = createdOrder.Last().Id;

            foreach (var orderDetail in selectedProducts)
            {
                await orderService.AddOrderDetailAsync(orderId, orderDetail);
            }

            Console.WriteLine("Order created successfully and is pending payment.");
        }


        static async Task ListUserOrdersAsync(IOrderService orderService, int userId)
        {
            Console.WriteLine("\n--- List of My Orders ---");
            var orders = await orderService.GetUserOrdersAsync(userId);

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.Id} | Order Date: {order.OrderDate} | Total Amount: {order.TotalAmount:C} | Status: {order.Status}");
                }
            }
            else
            {
                Console.WriteLine("No orders available.");
            }
        }

        static async Task GetOrderDetailsByOrderIdAsync(IOrderService orderService)
        {
            Console.WriteLine("\n--- Get Order Details By Order ID ---");
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var orderDetails = await orderService.GetOrderDetailsByOrderIdAsync(orderId);

            if (orderDetails.Any())
            {
                foreach (var detail in orderDetails)
                {
                    Console.WriteLine($"ID: {detail.Id} | Order ID: {detail.OrderId} | Product ID: {detail.ProductId} | Quantity: {detail.Quantity} | Price Per Item: {detail.PricePerItem:C}");
                }
            }
            else
            {
                Console.WriteLine("No order details found for this order.");
            }
        }
        #endregion

        #region Payment Management
        static async Task ManagePaymentsAsync(IPaymentService paymentService, IOrderService orderService, IUserService userService, int userId, UserDTO user)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- Payment Management ---");
                Console.WriteLine("1. Make Payment");
                Console.WriteLine("2. List My Payments");
                Console.WriteLine("0. Back to User Menu");
                Console.Write("Select an option: ");
                string paymentChoice = Console.ReadLine();

                try
                {
                    switch (paymentChoice)
                    {
                        case "1":
                            await MakePaymentAsync(paymentService, orderService, user);
                            break;
                        case "2":
                            await ListAllPaymentsAsync(paymentService, userId);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static async Task MakePaymentAsync(IPaymentService paymentService, IOrderService orderService, UserDTO user)
        {
            Console.WriteLine("\n--- Make Payment ---");

            var orders = await orderService.GetUserOrdersAsync(user.Id);

            if (!orders.Any())
            {
                Console.WriteLine("No orders found for this user.");
                return;
            }

            Console.WriteLine("Select Order ID to Pay:");
            foreach (var order in orders.Where(o => o.Status == OrderStatus.Pending))
            {
                Console.WriteLine($"Order ID: {order.Id} | Total Amount: {order.TotalAmount:C} | Status: {order.Status}");
            }

            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var orderToPay = orders.FirstOrDefault(o => o.Id == orderId);
            if (orderToPay == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.Write($"Enter Payment Amount (Total: {orderToPay.TotalAmount:C}): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal paymentAmount))
            {
                Console.WriteLine("Invalid amount format.");
                return;
            }

            var newPayment = new CreatePaymentDTO
            {
                OrderId = orderToPay.Id,
                Amount = paymentAmount
            };

            await paymentService.MakePaymentAsync(newPayment, user.Id);

            if (paymentAmount >= orderToPay.TotalAmount)
            {
                await orderService.CompleteOrderAsync(orderToPay.Id);
                Console.WriteLine("Payment successful, order completed.");
            }
            else
            {
                Console.WriteLine("Partial payment made, order remains pending.");

            }
        }

        static async Task ListAllPaymentsAsync(IPaymentService paymentService, int userId)
        {
            Console.WriteLine("\n--- List of My Payments ---");
            var payments = await paymentService.GetPaymentsAsync(userId);

            if (payments.Any())
            {
                foreach (var payment in payments)
                {
                    Console.WriteLine($"ID: {payment.Id} | Order ID: {payment.OrderId} | Amount: {payment.Amount:C} | Payment Date: {payment.PaymentDate}");
                }
            }
            else
            {
                Console.WriteLine("No payments available.");
            }
        }

        #endregion

        #region Product Management
        static async Task ManageProductsAsync(IProductService productService)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- Product Management ---");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. Update Product");
                Console.WriteLine("3. Delete Product");
                Console.WriteLine("4. List All Products");
                Console.WriteLine("5. Search Products");
                Console.WriteLine("6. Get Product By ID");
                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("Select an option: ");
                string productChoice = Console.ReadLine();

                try
                {
                    switch (productChoice)
                    {
                        case "1":
                            await AddProductAsync(productService);
                            break;
                        case "2":
                            await UpdateProductAsync(productService);
                            break;
                        case "3":
                            await DeleteProductAsync(productService);
                            break;
                        case "4":
                            await ListAllProductsAsync(productService);
                            break;
                        case "5":
                            await SearchProductsAsync(productService);
                            break;
                        case "6":
                            await GetProductByIdAsync(productService);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static async Task AddProductAsync(IProductService productService)
        {
            Console.WriteLine("\n--- Add New Product ---");
            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Product Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            Console.Write("Enter Product Stock: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Invalid stock format.");
                return;
            }

            Console.Write("Enter Product Description: ");
            string description = Console.ReadLine();

            var newProduct = new CreateProductDTO
            {
                Name = name,
                Price = price,
                Stock = stock,
                Description = description
            };

            await productService.AddProductAsync(newProduct);
            Console.WriteLine("Product added successfully!");
        }

        static async Task UpdateProductAsync(IProductService productService)
        {
            Console.WriteLine("\n--- Update Product ---");
            Console.Write("Enter Product ID to Update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Console.Write("Enter New Product Name (leave blank to keep unchanged): ");
            string name = Console.ReadLine();

            Console.Write("Enter New Product Price (leave blank to keep unchanged): ");
            string priceInput = Console.ReadLine();
            decimal? price = null;
            if (!string.IsNullOrEmpty(priceInput))
            {
                if (decimal.TryParse(priceInput, out decimal parsedPrice))
                {
                    price = parsedPrice;
                }
                else
                {
                    Console.WriteLine("Invalid price format.");
                    return;
                }
            }

            Console.Write("Enter New Product Stock (leave blank to keep unchanged): ");
            string stockInput = Console.ReadLine();
            int? stock = null;
            if (!string.IsNullOrEmpty(stockInput))
            {
                if (int.TryParse(stockInput, out int parsedStock))
                {
                    stock = parsedStock;
                }
                else
                {
                    Console.WriteLine("Invalid stock format.");
                    return;
                }
            }

            Console.Write("Enter New Product Description (leave blank to keep unchanged): ");
            string description = Console.ReadLine();

            var updatedProduct = new UpdateProductDTO
            {
                Id = id,
                Name = string.IsNullOrEmpty(name) ? null : name,
                Price = price,
                Stock = stock,
                Description = string.IsNullOrEmpty(description) ? null : description
            };

            await productService.UpdateProductAsync(updatedProduct);
            Console.WriteLine("Product updated successfully!");
        }

        static async Task DeleteProductAsync(IProductService productService)
        {
            Console.WriteLine("\n--- Delete Product ---");
            Console.Write("Enter Product ID to Delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            await productService.DeleteProductAsync(id);
            Console.WriteLine("Product deleted successfully!");
        }

        static async Task SearchProductsAsync(IProductService productService)
        {
            Console.WriteLine("\n--- Search Products ---");
            Console.Write("Enter search query: ");
            string query = Console.ReadLine();

            var products = await productService.SearchProductsAsync(query);

            if (products.Any())
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Price: {product.Price:C} | Stock: {product.Stock} | Description: {product.Description}");
                }
            }
            else
            {
                Console.WriteLine("No products found matching the search query.");
            }
        }

        static async Task GetProductByIdAsync(IProductService productService)
        {
            Console.WriteLine("\n--- Get Product By ID ---");
            Console.Write("Enter Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var product = await productService.GetProductByIdAsync(id);
            Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Price: {product.Price:C} | Stock: {product.Stock} | Description: {product.Description}");
        }
        #endregion

        #region User Management
        static async Task ManageUsersAsync(IUserService userService)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine("\n--- User Management ---");
                Console.WriteLine("1. Register User");
                Console.WriteLine("2. Update User Info");
                Console.WriteLine("3. Get User By ID");
                Console.WriteLine("4. Get User Orders");
                Console.WriteLine("5. Export User Orders to Excel");
                Console.WriteLine("6. List All Users");  // New option added
                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("Select an option: ");
                string userChoice = Console.ReadLine();

                try
                {
                    switch (userChoice)
                    {
                        case "1":
                            await RegisterUserAsync(userService);
                            break;
                        case "2":
                            await UpdateUserInfoAsync(userService);
                            break;
                        case "3":
                            await GetUserByIdAsync(userService);
                            break;
                        case "4":
                            await GetUserOrdersAsync(userService);
                            break;
                        case "5":
                            await ExportUserOrdersToExcelAsync(userService);
                            break;
                        case "6":
                            await ListAllUsersAsync(userService);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static async Task RegisterUserAsync(IUserService userService)
        {
            Console.WriteLine("\n--- Register New User ---");
            Console.Write("Enter Full Name: ");
            string fullName = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            Console.Write("Enter Address: ");
            string address = Console.ReadLine();

            var newUser = new CreateUserDTO
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Address = address
            };

            try
            {
                await userService.RegisterUserAsync(newUser);
                Console.WriteLine("User registered successfully!");
            }
            catch (UserAlreadyExistsException ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }


        static async Task UpdateUserInfoAsync(IUserService userService)
        {
            Console.WriteLine("\n--- Update User Info ---");
            Console.Write("Enter User ID to Update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Console.Write("Enter New Full Name (leave blank to keep unchanged): ");
            string fullName = Console.ReadLine();

            Console.Write("Enter New Email (leave blank to keep unchanged): ");
            string email = Console.ReadLine();

            Console.Write("Enter New Address (leave blank to keep unchanged): ");
            string address = Console.ReadLine();

            var updatedUser = new UpdateUserDTO
            {
                Id = id,
                FullName = string.IsNullOrEmpty(fullName) ? null : fullName,
                Email = string.IsNullOrEmpty(email) ? null : email,
                Address = string.IsNullOrEmpty(address) ? null : address
            };

            await userService.UpdateUserInfoAsync(updatedUser);
            Console.WriteLine("User info updated successfully!");
        }

        static async Task GetUserByIdAsync(IUserService userService)
        {
            Console.WriteLine("\n--- Get User By ID ---");
            Console.Write("Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var user = await userService.GetUserByIdAsync(id);
            Console.WriteLine($"ID: {user.Id} | Full Name: {user.FullName} | Email: {user.Email} | Address: {user.Address}");
        }

        static async Task GetUserOrdersAsync(IUserService userService)
        {
            Console.WriteLine("\n--- Get User Orders ---");
            Console.Write("Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var orders = await userService.GetUserOrdersAsync(userId);

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.Id} | Order Date: {order.OrderDate} | Total Amount: {order.TotalAmount} | Status: {order.Status}");
                }
            }
            else
            {
                Console.WriteLine("No orders found for this user.");
            }
        }

        static async Task ExportUserOrdersToExcelAsync(IUserService userService)
        {
            Console.WriteLine("\n--- Export User Orders to Excel ---");
            Console.Write("Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            await userService.ExportUserOrdersToExcelAsync(userId);
        }
        #endregion

        #region
        static async Task<List<ProductDTO>> ListAllProductsAsync(IProductService productService)
        {
            var products = await productService.GetProductsAsync();

            if (!products.Any())
            {
                Console.WriteLine("No products available.");
                return products;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Price: {product.Price:C} | Stock: {product.Stock}");
            }

            return products;
        }

        static async Task<List<UserDTO>> ListAllUsersAsync(IUserService userService)
        {
            var users = await userService.GetAllUsersAsync();

            if (!users.Any())
            {
                Console.WriteLine("No users available.");
                return users;
            }

            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.Id} | Full Name: {user.FullName} | Email: {user.Email}");
            }

            return users;
        }

        #endregion

    }
}
