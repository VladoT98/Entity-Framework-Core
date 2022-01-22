namespace PetStore.Common
{
    public static class GlobalConstants
    {
        //Breed
        public const int BreedNameMaxLength = 30;

        //Client
        public const int EmailMaxLength = 50;
        public const int ClientNameMaxLength = 50;
        public const int ClientPasswordMaxLength = 256;
        public const int ClientPhoneNumberMaxLength = 15;

        //Address
        public const int AddressTownMaxLength = 50;
        public const int AddressTextMaxLength = 1000;
        public const int AddressNotesMaxLength = 1000;

        //CardInfo
        public const int CardNumberMaxLength = 19;
        public const int CardHolderMaxLength = 100;
        public const int CardExpirationDateMaxLength = 10;
        public const int CVCMaxLength = 3;

        //Pet
        public const int PetNameMaxLength = 50;
        public const int SellableMinPrice = 0;
        public const int PetDescriptionMaxLength = 1000;

        //PetType
        public const int PetTypeNameMaxLength = 50;

        //ProductSale
        public const int BillInfoMaxLength = 1000;

        //Service
        public const int ServiceNameMaxLength = 50;
        public const int ServiceDescriptionMaxLength = 1000;

        //Store
        public const int StoreNameMaxLength = 50;
        public const int StoreDescriptionMaxLength = 1000;
        public const int StoreWorkTimeMaxLength = 20;
        public const int StoreEmailMaxLength = 50;
        public const int StorePhoneNumberMaxLength = 15;

        //Product
        public const int ProductNameMinLength = 3;
        public const int ProductNameMaxLength = 50;
        public const int ProductDescriptionMaxLength = 1000;
        public const int ProductDistributorMaxLength = 50;
    }
}