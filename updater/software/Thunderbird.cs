﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.2.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "606c58af0f3efbe834f74362c2c1ac4347c0fed8d67247a708c7bccdb8936041a86c6b66ef57205d29bda0af9ac9107f0039174a15b725ce639469d462525c45" },
                { "ar", "935a727632ee7263af85784476ba34067625d75cbc87771b4ebe9faea0b0b2137b28520476e38026ebe6acfaac13fda041e45606dd9206af9fa973778358c143" },
                { "ast", "87dd9ccb2efeeb537bc13913710887a52069a7dc4dd16b6f97b7ce864f226cb1ff484869c8dfe3c3dde85008e33782106f6c7b0dd0b6bba345ac527c1438d371" },
                { "be", "e01548f6cfad3853592cd3775416b86779868396016164bdcf021098cd74addd3bd4577d6bfdc2c0f0d2432da6583b729706c8fbcb13d774067104dcc110b1d1" },
                { "bg", "2beaeebaf3c9ba2cce519913173d8ac6cb647d02752f8b9fb1954281f87ac33d0d6ac06bf196b29a31726e1ec436d323f9767597af7b1e606fae8a63d5686281" },
                { "br", "d95114eedd6bac4d586013201728b5381227bfce3c5518cfd7f210fe8e0026d3e5fdc02ff734cb5936149327b862eceb8155bd3b0b5e8fb191a55520bc48783a" },
                { "ca", "9e64f089eb0b8a4c20bd3832cd3ba5fc947993c5a05b67f879f9bf278236bc05250714af3f2259bf15be345f2415677b0f200d523873b08c32b81d2bc6d75f64" },
                { "cak", "5c4a4b42060c8aa17021000a8f5ef3741ba67d331f45515cca11d702ee8b8c6b8720c0e64bf1882261ae6e4c069b6a0d9fd504d656dee33649a8d7f92af155b4" },
                { "cs", "8464087d2a53eb3feafe3d26c0f40c24874e6df4687920094cc0f49ef55a70156963f5a93789654fae6ad4d4e408c542e5d1a81dc42020f59534a8ff89aff720" },
                { "cy", "9edc2745e8e80a41ae6d65819959fb9dd9a92d692c5dcc1ff47e511c3c751fb043e170fa35b984cf666bd7e0981247594e974d25b73b1b9b062c3b87719d4ae7" },
                { "da", "fd7806de054ca592fd023acb73b9dfb76c046e70656b8cd998185b3f85a9c62b36a04bf033c2eb12b82e0765d6205c901a68220d7999939bc08f144cac007b9e" },
                { "de", "964866237d34241fdeff903f6037b90f6d91af74eb1ea47dfdd988f8125aafbda11b8c56b1c9fc12da14e5d1a5c54b67cf2535b619e704c4f4de184d06c4ab64" },
                { "dsb", "3994981976c9ab38ef1b2215b11731399e0d3bc2e987d687e75eb56573fabefdecb8cccefe4c0530c68b1acb6ee14c8cf9ec7a941b6111f5d659ff5fe01fe4e1" },
                { "el", "76fb90e624465a7c36ada61826097faa32173ae29cf064cfe7f4e045c6ffc625292258599883ade1471a266bd6e6961ee51aed719c4cdf9df3b7296babacb08a" },
                { "en-CA", "f3fcc085bf1ba23ca208a05fe0fd26faaa4b369bae919a16ffa72d49e504ac595b7fead979a7498a893a129e5f41afcc84efe3698ed8753e31e5af3e047e736a" },
                { "en-GB", "890718594d6d6d225cb2f386d18cd86646a5992d0dedb1d248486ecfc6c47146c2ab5a154e1334e882c822f1858aff6e41e1df145d5e1a9e48a4e305b34de256" },
                { "en-US", "572743ce3127f247507747d15886486c705f4318ae75510e4aff3136a6960fc80bb838c625d4422ddb596a08d05d61c32ad8611bd995b3c3d3e640b8e68d37da" },
                { "es-AR", "6390b4be175d7c77c86787e72a4ac6b229965db04bee9fcc56e66f31fce9ca9d798d15b1fe94c7d47f9e6ee8d642f4ca4fb9352af0646931163c703c0564737a" },
                { "es-ES", "2e4ad539e72701621ec7867b52820fbc18b35c8f55902369088953fb7e025875877b1d00abb0ac4882d9fc011d8adbf9ddd32f8696700655221655ddb7283495" },
                { "es-MX", "9241ae5fb3beddccdb91d9bd3f549d64c9534369e4b8f016675a853dc41e99ee82716c4f77e5053262a13e4ebfa5721ff3f76dd5755c58100ac7c232cfec3fd0" },
                { "et", "454d4b36c106522ccd11bfd7201c01377958d9f8becb64899f18147fe4284101befc399f5da4d2efb1045b1a4b58d3ec73805e0f92f9ba278dfcba44ee7b0299" },
                { "eu", "529b44887eb5373961789f9153f8444a3f1d3820a9d451b0da73f14f6b970c40bf75f70a392b56814e6393dbbb8196fc7ca2ee72fd5871d29f1f0d4f1c212abc" },
                { "fi", "1d5d5c573beab050a5013aa20c4aaef6ff7b0c2f88d3effa76a346fba2ed81f56998ef11c7067ed9ab532dec274b6d815d75c8c553d29272f4551722fdcadb07" },
                { "fr", "cc5c7fa8a822f4d136af5eb1981f9c6a47dfaa9b8342d2ef918120643fd7b596f829e36aca89a278443f1195783376709b80cae90b77d56409317801d4e6addc" },
                { "fy-NL", "4143bef303ebaebcbb5713a211e05c5111e7417e15e3fcfb810d6b2c3e63d0f6d87134871e9453b54bf75fd038b68439dbac39f51169081ded36fb5ddea34fb2" },
                { "ga-IE", "38350de40cc1bfbaa02cc2e43ea3bc23ea0d08bb3130c029da90b5fdb4b333f746a6b5124bddedb63fdfe436c78f0324af81b0bda53e47e715707596b1b4abac" },
                { "gd", "a3b311cdb568edd8eeed16dfe34d5143fa6cbe3812cda6f6ed983d09a598af2b0f61d38c23c94b3e2c0dacae4ae9c0021a011d7510cecc5adbee243207cad121" },
                { "gl", "72673b1fff465a60980a1429ceb0ce4a2ed416ca0fa306fe7a459853a92b482fdf3872fc384175ab66a819f43ba0cc870e61f69d670365fcf74b94f15c1c6217" },
                { "he", "6602138fabc29e2f5603db880068cfe96f9e01048bb03af64e9c366b62c79ad02070d46169d76b80a41255c3187f62e417f8ef4f3e12fccde78b5a879d46a0c6" },
                { "hr", "807c703e66d3cad4c8db4e93e5706876a116a0ae0178caf29d56d5dad114dfdf62368cd83a6cea947025d5efb8f96381164a498c329f64b6d35905f19d0a2fcc" },
                { "hsb", "5815daa056e3089fa7a985f999b42cf8a0110b4f44c561ec0f053c127936abaacecf0aeef426d65818b5ce240a0f23fce7f095b149e84970d3253d5f27548ab9" },
                { "hu", "487d426c28c436e508d8a0034396061c4fa9c0613905c263be55c52e89143da6ed985c25a2df7527fee58f0dbdd7396ed3d92b447e6b79c30d4c886da5f7441f" },
                { "hy-AM", "24fa3fe2efddc6d16b727d1451eedd31ad68039384391c0ad232061b0780a7e0f603c5f9ec4b0d348dcd5c2a4d03788c3281849c8b4559c7cb82198dbcfb6e3e" },
                { "id", "0e46dbf4cecfd32f9f37edf6c98dfda4429969716fb90825b127bbe56dbb2fee32c588cf86b831e0475635094b156f4fc59ea41cca0fb9ede664c52985e4fe15" },
                { "is", "0f7f917da548f6e08ad52d710b1e8a2385fdcc2fb334289d19f4dff3f16bca0aaf52e3e034faf897b4744c2c3f119fd2dbf741bb7ad71e49515f70f0c9c4c50b" },
                { "it", "d72e8363f0874009faceab395df88cb47b1367922f208b797957b3d2549b761cf231bef051530894b876da38fe032d903e8c1cf88a7a0f4974e27d617e7f2c87" },
                { "ja", "80b5b9da694cabfdfec0fd210ec09e1e1ffa5ae60654edd59d95fa95f903179dcd34af9845491aa54cf889283edff614f7fe40a97832f39b2f02fa9410ccb0a1" },
                { "ka", "a34a414bc246a4387f8710bb30a2e2bea5590dfa424ceff7cb5e91c3128a90b874e7b0a755be82005620567066283cdc2d421fdb51ad377c2173587fb4109da1" },
                { "kab", "62c7ad526ca501534d3e458a2e6f53b53a0e5c2ef0f2d21ee81b38f5bf4d3acba1cd34da6ae91fbb9f4793dd2d86220c68b9e3c88d18c1bc68d79056412ebf1b" },
                { "kk", "2bf66679f286582983bb335197f833c1712f16ff2a9d48cd3919db8bca5280efc7ada2ab8707d7f3aaa2adc02924acf4ac7f9e1a491246dcdf78c7e8738fac44" },
                { "ko", "aaf02020508ec77ffa4aa35096165ca1d2f1cd0e3f3ceb39af8c265ee6a959d7e1e32a1c4bd86e184e0bf94998fa4aa98d50a3c9d12c1ca4d855de5b19127560" },
                { "lt", "5d5297700f1d85c3960679f27881ae5da0296899b91818f5968bd2be3b85eeae70f7685846fd55e9a38c53d147c38a08ddc1f22f495f0efeb484993c2c8d98ba" },
                { "lv", "281d48a118f1e7e3f89b9e14f34941bbb09ef41b81bcc1255184a349bdcaaef2f62a7a4220066c9a1190670adda044c079d6a0a045335e17be8d7f1f166349d7" },
                { "ms", "59696f7935c1199181e1ecb437a7e734f7cbcdd3b770648a7263a2532aef058d74b77d2313cde88fa7b80ca8c8be5b478c4d907d61a6e12304b03356cbb48e48" },
                { "nb-NO", "69cae568041b983b1a3cd0b7ff1c29933df17d42ebdd7601c4813891b0a0ffa480bfc1759268f2a096f64da219f1fd95abbf7e7fbef7567e757ca56eae76a17e" },
                { "nl", "1e8638ec10924b8cc718303fbbe1c699cded1207031fda66e8305610e911e5e47fdffde4863cff5dd7250ba9557a1887d86703823b20c34c5e29c2ae3294d490" },
                { "nn-NO", "80eb6ca43b04a010c1c6675aaf60d23dba09249d4d6b506a7e9f04a3a125259b3eea097ef177411b28cac23852013e31c93131dfb78f320a1cebf78c9d0cbfaf" },
                { "pa-IN", "11c0c2b3d53b06d7de99fd47b45c490a26a7124a5ac04edaac2a39ad737a401af0e46029f6493ae1ca175939d30713a3d91d2264d46802060beecf654685f0e6" },
                { "pl", "391474fce39b9e697c2e7a38483a581dabc67df556544c8e9cd4f498da378e2c99a990906404bcf84d6cfa2b4a051be7d4eab2109a73095ba8de5ecd5f97780b" },
                { "pt-BR", "f6439e0a4b2042ef6922d200b61365564166f18cdc6589c76a33b343d2e886ebad323122c65129b2202a270fabeb6888e3f0ff6af08fab07d40ecd32ace4bd0e" },
                { "pt-PT", "259d32e8b964f606b0714b76e794486f3231727ffdc4f1015a51d8cd9a3d4a28ab7d503934b21f65bdb1e2edde13bdbb941916251714f878728cec88c0345cc3" },
                { "rm", "304a909752977d37c77b50e1bb511781810a0b6b0cf0caecfa10fd99ef618b09926c6709a97aede7fbdeac51b543b28620fce8d1f53c4e2dc0c05d23a4521f90" },
                { "ro", "91edd9a1401832309a62c9cf0f9f171f47a2d75bcd1a3bc200d9f08685c20f3045e75dcc202be6dbfc459f10008398d62ec93cbfe186b5afb776da6ec5fa10ec" },
                { "ru", "fb6120bc9cb130f0943c24be629fddd26913426693511525e9c9934903865ca3c5a61bc04a9c7f925160e1b56dea858588571e9bb6432b5a6406f30f8a71a595" },
                { "sk", "6caf62f452a6cc766fdc32fb77eef567c7756c90df04cdd437cfe9a8d906d62d545aadb7a08a08df1b2e4bad74bf56bd38753bb1e5207d347d07c9b28985c4a9" },
                { "sl", "2da643ce60745fab1248b4b87d9f1bc38db857d2b1f5a86bf4bd4d0d2e06087c621f3ba08a1e27e83fb8a958377e269d2e51011e47ecfba1fcf223b5c380eb19" },
                { "sq", "fca139a091684886254b1e80c003251ed61fb5258109ebea9c54b503df052b939b9a3a88086d4f5e4f2a9be26efce400430fdc431db392ade78da994c73dd10a" },
                { "sr", "09af34c93d8938999c02e6d660ea9e4b0609cc05c5af693c4a2759ff9d539566fba557a94dcbec47ba30fa15de1061f617a6eaf3c70036b5d737a09c29fbf85f" },
                { "sv-SE", "638a907a41b6503db9a43776182493c4d37c179f772bfc9d8b449bbbf1cef8be89a703b3948e5f1f770461c4fe4dfc15dbc73de540685dd2c9bb355e71bb1b04" },
                { "th", "148336ab7e3655c51ec01d6ca5b13c9c9fe182d8ca1388f954c1aa3f0bf6e0ce435150399c1c54fc07798382577de1ffbb09be548d6e3d13590b85c7bb54269c" },
                { "tr", "83714e041a7e7e970b2bec0edad872c8e00f75748c7c3a0603491dd441bb256ef95d3aaa93a37ec458c838b8d698f995a8e264f4c98ad663601e10760d45f53b" },
                { "uk", "97f8853abec3267a10df30dfcb82f4ba5a7dfe458a515f13c3fb299d0b51a577b536b6ffbebd11cac48bdb690cc5ddfca59b5f1ac9ecef8f70b80d0816fae24a" },
                { "uz", "14ed37721195ca46bfc58d1fce45ad5116e20fd1758b2db259f10015745e2ad2de25484d19ffd41cec61597baf5564e571762a44488675590c615a03a8f0c1e5" },
                { "vi", "c2b8025db30da9f621688e84e4cf81eefb81f31dd6892b632695f5ef09bae1396e18f48c65afb8fda99bf091581b302cc2d2280703dae7dea8545f5676a88ef5" },
                { "zh-CN", "cb2ca402aba420fd0d2614dcb24ee90b35084fdd76b7ba1defb35342b4743c28a36f820b9283e3bd61133ba71d2e31b9f0c89deeadd3aeae58ba73e583f08a5a" },
                { "zh-TW", "02d8781b449257fe343c17d780c29f0a68c1b39881c71576d11cef9e5794d4f9e7ebc6649fe24dd1225f236ddb5441ab3ea7397669688fec8dc1c49a3eb1d042" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.2.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "2882da70a5230c8ae6aba80ce5a9f726e3d3a6406b49ae44fc860fd042f931d89f2ef6d55a8045d2f3d39a1509013cba94be416fe60b8f6957738f32b5c8a2d2" },
                { "ar", "2deeb49cd56fe59eb067458020f05f191cb52c649c42347a69c9f2ce34d4ffff5ec2bdba790f497dcfb4013f2c5edf0d136be2657be1a1ee21d1d8b99fe8260f" },
                { "ast", "ceea1116b668f010913226c15712b1ec6894ad85d3dca97b5aea5013677fe47d56ba798392353b6ae120d11db8527536bfb769683d0a33d6bcd6a4778cd35b16" },
                { "be", "5b4cd4f90d2fab828e4a1eccfb3238db45d9dfc1f31ea44ba50eff9bccf55caed0b1e35c7ea95f78875770a9798fc4f1f5dafffc63f86db32bdb237608cb0a1d" },
                { "bg", "2bbf9d2029be06cde35d569871a5f7521308a3c63c9999fb59e225bd4d2a66529b3b4968753556ac92a2fef561884ffd300ca66d2c26f5c9b5a9514463a891c2" },
                { "br", "ba02df535cc8060b5e314937d3356e6c466be82bd172662bde460ea9b450d28a3125f70022df06fb5c3ed65a69394382fb5c984b9fb71c18a259f7da05b45c24" },
                { "ca", "08af129df33db4f1aa9f753638737020bbfe325ad46782ed645a5e4dfb98fd5c2a019ce4391032fb45d6f81254ab2782c2cb3354d3a8663e7b14cd6c43d14fde" },
                { "cak", "49d7efca46cb1b16437a9697f588794256ce125ccad83626d83736b7f119942ef251d63e542acb171abe8f814f5ca51575a18fc6ad5ca164a27befc391ba1ef2" },
                { "cs", "a2535fd97fe8e8c63afbcd3d30e1f9914a788c3a4828ec395901426f1f57cc4a448bb4bd1c1880158838fd53b38b0b61455b3d3ef4b6197cd91b445591bcabed" },
                { "cy", "f8fd4a76b5f6c377933c198538625d6a221742ba392fed76bac1356ee245a6505129e3e75eddbbd08d405c59a217784541c421998b0b397fc6c724a842b15db9" },
                { "da", "e5144e97b09dd1c61b6cf8c647b8b398011d3d68e0e37bc017bdfad8d8a6e5f58c2ce74f38fe3292edc381d0fc81a7c0e8881625468d877ce72df301616f95be" },
                { "de", "7b967ae98449f2f2dbb6a083588ebb6f26f8865164ef32e475d09829a31b28f23cfd699cd42884267d7fae778e058c2b4ae2fc7bdad85cc47465e02b0c31b472" },
                { "dsb", "263215f8b2df8243c69187fb1e9657e272240a89f285220e71079ef064d4a7ecfb010f5033f99834ec457fdc77c01e3e444de34ccf7753c4deec493f838f1cfe" },
                { "el", "231ad308260048cb5a1e01094e50d7bd9a6cc1d050561f9c6eaf56633e9011aa602492ccc986bd55b319de406368dda009732a9a095c67506ca8ade7ecccee49" },
                { "en-CA", "4a31f99963df918e2503440da8654afb20614be570520e8e2b6c1a87e21f7f5b5aef4e93e4575a6507b5f403c052c04b56d3e47101666cf1c87ca93aa888aa66" },
                { "en-GB", "948238a7f19190c968cbcfe9f4842598fb4025778c2dc2f9d7920f00f9533c3479e81eca0124feb793ba1310dd6f1657a71ff272a818f75e98af18824cf0da4d" },
                { "en-US", "7f6c6365ffc80c1d8a6ae0a819aec8d9f427d4239e35aff2104031bbb9d54e1836fefab65d5be811192c8bfce7d0cfe9b66472200720b2880bc4dcab6f8ceef5" },
                { "es-AR", "e72b04af9af06ed269080d9dcfb29a32e87c71f2c1a20f10750478fd7cf96e3495af2b9dc6b9220137f20344dd322332b98a4a53cd7e31ec5ad6fd3d6b6a698f" },
                { "es-ES", "1db9565c62a82c4dc30ffef69d60b2fb721cd194e8f3be1f26ebb3f5415f58db2361bf22b02e43bd88a63b19dde75d81fd8348c22353bdb8dc94a17776152bf1" },
                { "es-MX", "89f216b0295eade6b3cefba5a2988486c652a41aa644dc629e28af0d6baaadf26c337078ee14d844bba4292fb38c84b7d42cb6c2ba0add1e0f26d33372ae48f1" },
                { "et", "e26f9061904947452b34dabf9df26dba22e6e1b96344ffcb29e6e4671254473d0f69cd0f66e384c9ca95ac2091bba1959ef4398e679906c81b5e23494bcff517" },
                { "eu", "c8b2d727ad500b2c495e71d2bba97af0fbb1445d4fa7ac86f0ef2c24b4dff67c78fdc4560c7bc898d8c0d5d4ed2eeba3d571c87f1a5b4c9fcfab426b9c402a84" },
                { "fi", "9798414dd3e0830a23afb8b43c5b8a4b1b70284f8752c8c3566687d04661716e439873ed7db5094455f185cf09b9c99720593563e0ec8a11c48464177fdf488b" },
                { "fr", "684e029546f8b52ecd5be07683f60e8a4535e5291d30816f65794363bc74e130437f9d16b5d85e7265d3a6262b702d616d198593ec804ac8f9a0e1274a35fab8" },
                { "fy-NL", "0626dfd8ea2ea57cdcaa414f9a8b9e867764093b8bbd85e35f7ffc221491e2a9e6bd49378d29b291bf23c755ada55edca84ea4e6ea2b85f3c573d8ecbd41258a" },
                { "ga-IE", "709cfb46791744842a36ec59a9b5767ba921f32a5a7c9d16a6027268f37e884db52345ce28c1885680a9142008bbd39a65160a4af90f15553f2d3191f40c60c2" },
                { "gd", "63f9affdadb414db73c01f647d2dec7d1fee3b8581acd2589aac5ffc484c0670b6b4fe40a61a180daa438c9a81474c592514fb264cd0639fcb9daae88f0a5a1a" },
                { "gl", "0e1b6ec78d65d57f6be9eef5c32616165c30e88cd891a7d4d412f57b28e15143e120a80c0f5e4b99e692969cf6c397345ae809a1340eade07f6858357fcc2756" },
                { "he", "850a2c14caf54e41f4de2a040360e65a3e0459c418e38ee0f299dc25d9f7f9cc55bfa9b2b1fb548e560439e9e18fe9466cae17f7989d098ba785643421950e17" },
                { "hr", "4ab6acefd12ffa41ca5c82b5b3e978881334753a8ff6c3b8d946eeea0371ada667453f4fae2243d1983f9e9a593e4c2a6d23c6d6da27d8382dd48b8f8f679441" },
                { "hsb", "203c117e1070f4f74dccb84c26b29cf62c0ed014a5fb27c56bda8946f9d5097808b9a5abe92e522fcbeb7a5953508c4e7746ede602d3568f64769ad4624fdc78" },
                { "hu", "9cda429b22b02ee8abe42856d6bd18368687485f3e50f3c77faad5ab45b510c9a5d005ff5d6432f8e1745ca83b842f76afba62d84e379fc4e75d3702a6332934" },
                { "hy-AM", "d4929b6a87ed5c5da1fc3ef06797b7788a6c97d2eb39e791891aac0560b2f6161b412db6e23704145fc3f2a3f2775b9285e22f8a7429b11e04d431d8544d8f8c" },
                { "id", "bd1d66dcd2f7d8ab744bdba19ac635a2ccd543e3c1d7633c3b793f63c86eb835056123936661429f312b5f78516b55a9c8a135d9fdb8e2cc4eb84b027984c317" },
                { "is", "00d9ecc193f2c6590b7cb0e8170106de13c58f31d239837b671defb390fb169bdf0a98f2f103dc9145e2e703d8e9464f12f931297e833c2e58ae56921542d93f" },
                { "it", "b3ebea8eee7036875f14bfa75759d8e3b1e65144b9096943aaf4d7e4264c7069d50aba3122561896a7bcd09700764590694ec5f8417cbe36a8f547a8e42a35d8" },
                { "ja", "0b7e61b53f9c237f9ed929c4645595a802407fe9a3b963e866bc249aa1aa04761a968fdac9de2c5ea6d30dc93aad3c8aee88859538890a1ad60c39ded439e996" },
                { "ka", "3c354848926ba2848df448f1ddb533ecd9f116e7bd9861c51e8616531bc85f63fe442a5eff15c5ca28939a365189df49324f2e2173b917b25688fb2fee3367fc" },
                { "kab", "e8d193ea7ae4beb04e3b137cdd77f84d1388f33c20649a8b32783fdead8dee9b1f57952c42ef76ea7b15e87cb26ccdd0684dce0e6988af5b8877f076cdc27e51" },
                { "kk", "7e30c57f6ad51a9c9db907fe0b9512c4c9509ee19a9ed119b712033a79657bed2db2004f5ccb7055146e3eb41c6f3cee4bf178a35c9ec4a398239d0b5cb34075" },
                { "ko", "0d5427f78bf4c73661a478df56ff437795928b6f239a0a8afe014bd11fbae0f5c4865ea1a5a78c4373334c0577b3141a902039719aeb5028ee1687e6da2c62c4" },
                { "lt", "6b90e49a0b688f44b90aa0c11d5a573b68657ba50642288b23534ce2433dd9ec8741ed5d9ba1c778c2e3a8aa86e5a78b1690a60f37f0e939dc0690ab023e2615" },
                { "lv", "dec61706dc23b793c5d1a7dc0ac1e8d904f8fba5433ba6cbe5a48ce2ff5cfd921839ba5de0797cde46926ace41edc9e827edb19516d9fec78ac815e89b5b09ef" },
                { "ms", "31376f349788e6952919b40204c45ee335538220c3137198da92969b139f72a94a96cc384b8d3486d8b8a8cf648303f9930b299e0c80c47dafbff41d4980336b" },
                { "nb-NO", "58658a775ec6b8c7f63740b5aa143eda637821819f096de2d33de9cf102688e15ecf28c2c11f71e2c01f22cb28862e899414e4e50cc4845f53154f08fa7361ca" },
                { "nl", "83852bf1e667322c0e90c56cf2d44f5f9cdccf88745aaba0b7e33e6e9494bc147a5ce5032f9639cad5b358fa621ecf3c6e623868c92c57e84df1795c24a5c74a" },
                { "nn-NO", "1bdb8bce5c1e42c26fd817d85c613bdc81fc273f0fbe1cffd2baf51636f34a659c545653f275018a8840cacad2db080ff27a7fe4f1ef400d6f22670ea1688375" },
                { "pa-IN", "06994f7f36ebfb16e2838170bde9ff24d05a5c763e2250f0ec81c385e8fbae430335c9c3974b3eddadbac0a4f50f6d89f7d8a034498aecfff5c31a33ab4e2537" },
                { "pl", "5ca15133191d931b0b713e6a578792cbcdb081a15c1052b5005c04a57235ee4a64f68e6e289500cd737345e11e1a94d072c76a165ff02c8322981ea2788dd22e" },
                { "pt-BR", "d2a4795cd26b3f03e6fd1509212e6c5743a8d446d050e840a13b0d96190d5765fdc2b416275094c36c07b93452e35ad4c4a9f7652fabfa129c5b96792699e2ac" },
                { "pt-PT", "2b1e6b3d4c63ba11b15cf081525a268d3ec1e4c63f6f3cd06f3c8a94023aef6e3275e40cdb4093acbd6acd409efaa512a4964cb590f8eccac4c591b89db04ae9" },
                { "rm", "5d6fddaff9dc29d879e9e281eaeb6b4da2bbc0f929e87ceb3b6d6c4c08d2e4ac750d993c88aab7c583699832d1c7d9282ee2fc4e7008a48fca163243f4d67f90" },
                { "ro", "8bd4ad189b24f10b677cf3360cc19085b42fb1acbac27b0de1c24c7cb11c424752aff4c312c55ceacbadafd19df748d5c8ae2f042d548b23319b8f4044c2352c" },
                { "ru", "a792122e3da7cbd955baef8fa5c15323c2d7278bfb897d5b31dc1596cded961852086a40b07c8812d33a4a18fc646be6b4315dd00f479242990e1b027d825802" },
                { "sk", "9b839216f3c9727b5dd1c555b7788ece57450814fee14b39e2e67d1106d61391de95c7fe09175c6d8a44242bf69e1117218a7695c41d2a1ab2bd73ec68bc3eac" },
                { "sl", "8d721c9bb1e08acf314fff36e08e11bde730d56c85e895eded63eeb8c3a57ef93dcd4fa352a97c3c32beeb66e81bd9800e69e1d9aac532fafab0f8d7dc17fcb8" },
                { "sq", "d2ac20d97d6d861d4edd8de99a676399e18132a6b8e8effaa4a70ac18da8b4b01876ec3b3adf7bd932d3e6bab07221c0c60a475314b60f27254ae5a861bca18e" },
                { "sr", "a05f49aab85742c4400b9e6346987d8553ce9c678f18376c3abbb4b98767926844b4e5b5e2d5bcc0945054a2b4df24164d156021f3257bd5b937c7c54d2e4d98" },
                { "sv-SE", "2557e352af5e4e38d39ff05c6057af93b9b57c97655470ce292e1923c46c6cb6cfd1c0125d611946b2fa7fac6658e5ed38eb71f9decb7d27ee3bbeec2e22af2a" },
                { "th", "83d5e23e5f27de7f7de1fe4c6594902be74aeac5187772746fc843b39f3e6cc1d5b28286607f74f4ec07e387ac55753e6f8669318c5480d0679818cf36e26815" },
                { "tr", "ebd6e6f91e3bd26624c5b43d5f5196e904b03724b8fd82f043f900ee767e8216a985cb049d47229c9fe9ecabad311e92614c0fe2bea3609ab8b87105aa016600" },
                { "uk", "9d9d70d54420dedd9154d7823c08bfc8263be986762ca0076d355b1a02c31129991a065ddfcf1f42273321a0189213b3fc9be298b02f50203f7c6aa12e736189" },
                { "uz", "cf137d24b10aefde0769e33d788f369ea67e286afcade8697a5fdc4446d713dc7afad00f4a4369c324b7da593abd05dd844697d652a5582e4b0d6ccf77afc68a" },
                { "vi", "ab41a1bcd591ac3a9452e065f9041975b9af638e8923724260bac247b95e9d0c505d5725e45efb713e28eb02479cbd05de125b0bda59fbc4c66283bb22065460" },
                { "zh-CN", "db79a10039839366da6764ccbdc427c3f1c47c08a45fe9c6b4b687943b7e2e76286e01a4a12ac11426d5ea5d8e07a21636aedf4bcac1df7efc75fb3ab0fceb05" },
                { "zh-TW", "9614d0cbb75e60c01e7c75663187c157e5b1bb9d743de8a0071cf017f41ba57a31bb08ec1fca5e53a05c954ca978a1a72c929cd4e4827e3b8cce04dcdc612fe6" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.2.3";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
