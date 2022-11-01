﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "0886ca883ef394b56972e5420f9d151635cb890a6bd94e107a210045b1e2661d80c68f7ea27989f6e1b7add0b8d8079723363243286af943569479331feaca34" },
                { "af", "5d8cf13d060f102e7eb5dfe3db609c88eb37e090612942c3c52740a1072662786bdf89e6d34dd90483d4843a601932d5bee12f0bde8c6f6e2d5ec812a789d977" },
                { "an", "b1df5982be2d8d0dacd28345c875c260c6d3dbe33575ae2f191cecba16397d7544cb4d2c5b0c450f566241c73ca22bf0e3560b10648a1daad18d6ab4a82b7614" },
                { "ar", "cd7178695b4a7b86962fab8b09c73bf88e6bf6df9d19cf8fdedd190f4f23844a38f41310289567cd6a67900edb15a1ff3d3dd4bb3008ef2dbca0a216f0f8dab2" },
                { "ast", "66c3fd4df20c6b1427d7f65812113485c593c51227a08dc2acf60a129d24e7cb8ff5ff39d099219fc63c50d3dff6c05f2124ff420ee92b4f1b51f54dad7dfb9c" },
                { "az", "43e60f0c29f26fb2432a07966aabf5d37a7b578b92a2b61133b85cc9fc21ce50a7cb63ffdb1c431b3bfd6dc04f4c661ce8e42aa18e63ecde95918afd369c73df" },
                { "be", "9fa676ff332db32730c326fe561c666c1bb827b4dc67abc6fd2cb03b6bdc51195add43c9caea6e7778da219993fa1e0d971ce426212cb4c00078f1cb04f854db" },
                { "bg", "4545e211205a192cf597c2493e0dbef7ff62f74b2d75bdbaa54534b918ddf735a5878f3944d48de1d406e460f51e3da21238f3921df63e89c6a90bcf4c3846c7" },
                { "bn", "da71ae3e6e066eaac98782108a3933b56d58ffeba92879421e29e1efcf7b1da8611d634eee33ca5baa1eb929336a0764c85c7a895c01fd125b6c5bc540c9680c" },
                { "br", "0bd0f623ccde578e5deb7db7fcbfc789d24dc0959a706ff196fcd804610c517c3bd6dc01be75a9f1341fc14a4dc69e242d6e5d8242c36f57b4881b0a67b567ca" },
                { "bs", "7e30e47616d52515125876403f931742a75b4ee86e481c29507d0a397f5c04dea943bee403a745d3fcd51546276309461880d32c10676ed7d09e6f29508dbfe6" },
                { "ca", "4b116812e9d5b3d771c067d68d47601e95f1b616d953897d3c5d7ba4feaf0b64297d1828cfab5167b3d64604dc98915a23d148de408210000f4273147c763827" },
                { "cak", "d6bbecf19b5961eabbffcab991e85e99f1a6b0050b568f08c3b81df080eeddf128b9a2bc497a47a2903cda97b29cd0fc82dc5dd777c174cb2422fe548d61383a" },
                { "cs", "2d3ff4cb11bb7e287d74820cad5facca38b29869ecf76ef02be5533ef5209f0f524df113cb1f474a7d28a3e54e3d214b6b464ed5ce4538d0f2ed5ae680efa3d6" },
                { "cy", "173b64c691b073b8c80f102dcb50bdeada7b6120c99c77d6f2bdb3ac1bc1d4691db2df9b5b9cb3da8c087fcca29eb086edff835e170f6bfe78d113cf22d78a12" },
                { "da", "f4e80752fa12df1bf5fb525c3f5c6c2d8915b9a6aa4d5d96acab045456bfafccabe6c8668db3e3fb47a09071f59118d11d23d41040a0f254f8de5b9d13b458dc" },
                { "de", "0b40806211a57378cc065587199939dcc40dc5bb07d4e68ba612ee84b23316eb7ed4e9bdd7fde8cfe5c37b32aa7df9c09c3f52c4359de5e93d93308d6f91e56e" },
                { "dsb", "1cb21b6dfb3c0f462cf2467790fe162dc02d1cd70dca3ba71fb15bb9be85125774b236cbc3ab97bff638da8bb2ac49acf9c1f30d11cef30d2df4804a580e1dc7" },
                { "el", "1da8e52320e726ec708daa5fdb709ebb3b454d43afff470996ea9c411a34cd40998ecd8adfb69a55c41c64468cf804479def8c6821049c1a91dd73a8faa392ee" },
                { "en-CA", "88674bd4d48e99824a979f9103fd00234a9b420a129e61c760885704d1dab77ff7c15083ca60dc5fc53a72adcc7ccd804602fd524d87012873368c7c4287e207" },
                { "en-GB", "b485bfdcf3963fa07b55cbee8c7acbff73274919a4ddc9dcc580f28f25c27494af5a6bf7a88051c8f80e7e8f80d0c98759cecbdf016cdabab4cdad46770fdec3" },
                { "en-US", "4c29e2e488d65b2fd1946be5ce7fe0d3a2c14b9d73b35a072394ed550b5eeb454556689b17d9e8f0e9ce94856b9576554aef857fbac00c4ff047cfe395a8c5ee" },
                { "eo", "d07f5ef9b456c0ae8775c6fa21b55d8f97c7c826cabb09eeb418846c89a15cb5deb949abe7bcaa103d92c69df8c18c6e1dac8945612fe927ba488d3de6d85468" },
                { "es-AR", "55dbeacfe788962ed71a1d99db5250cf60ea508683779be95cb0124b676fb7b1f1ae2165ecd6ee4ed94dc67d49c917f526f025a2b09ab6c130390fd985ca6d39" },
                { "es-CL", "f7b891fb93a13bd671ec5cc024d5aef389831c91d8b52844e0a5d9d1bcac04fdcebe53a3080462c68f425e407979b29e291a4e2dd7753338ba6b7a3e45351471" },
                { "es-ES", "eaf9b653ce13a6c369208d72b76854dbf9217f399784ed558121c0898e6dcbd6539b6ad9a5ff5ee2dcfbd1e9ff0a0ce987946147bc76456513e5946068a63f34" },
                { "es-MX", "00e2d0ca267c5db092b1a9da59211688406a86a0e97bb92179ee4da170b35f6543b84e01c21c0b362ccfd6dba33145f05b08c1b013239a7852456f5bf7c1bcb3" },
                { "et", "79b7b34d79128daa3c9c5e7919461f3ac422b1614208aef804b3058c6ba16ac1adb08c15952801d66779a048c8e8d782ff99c8217524adb77e48b239c7ac59d1" },
                { "eu", "e7be56624697d4be24ec891429350dea64a28bed0788b9dbf34a52efdba1242aebc123919e2ba6f6be1891cd099d17c05fa7ceb25d82e2982da2999f0c3446b4" },
                { "fa", "6dba493b8ac994eb3d3d2bde09da9b329fd06500396ae74edfbe357a246b73f4339f5d30e1007240020327e069e3f9414ed93fed0a07c43d60bb7e5226e3b3bd" },
                { "ff", "0b44ee93f78fc7576b16d83af4b2810cda7529bc185095b3837315e449ce0b220bfe0092aa7672be4be65e640bc0642719aa17d0fb48a3e5f87965fd78357ca0" },
                { "fi", "3add31fb215a64374535c1d113b17f4b61920a22f062df4ce935ff099f59a8b980a0e8f4034c6d08b33edeb633d7792e6101a077c0b928be8149cdaf002f8474" },
                { "fr", "ddc18a546f9de63571c6681758017340971a601433c0c3e535abbafd622bc58806d97af014f0c6b41eb3c64c676ee062bb8152b83c907bb364c83d196b120aae" },
                { "fy-NL", "3ffe05191e89bdd8ae645004af849615af40e248236b44d9c39e3cb3fe21fa47086f28b752acad4651a669c0b5bf5b8d704bcfa53768706ff38b6bfd414d4466" },
                { "ga-IE", "18208e6525de5207446734c4e0d82d0facbbfca9100ddaca774d53c26c76abadf864d3aa36ddf1a9dcf7f0f7e393529f158cdec2b6958c011acda90bf9776897" },
                { "gd", "28eb760815de646c730101d9227d9ee0aa830626ed0b885a70483d687f7ae0c5a2c185cf71bb7aabd8757e1013cc2045b85fa56069f89efa17476c7a0ff50693" },
                { "gl", "91f535ef5c942e6456c5c42ac572a10591ace23d38b92c0e898df85672177860bf53c7035a0ced14a7e6a22821f8d102507532faf65536cd50ff476cd13fda85" },
                { "gn", "ad333b78b8636738efb21a74df23040d66f90dc84066c44d349c23c22d8b7e1e2b73f594632e2aca759ad93abd5ec2ca3aad85221e00aaf3a1ddc72230e700dd" },
                { "gu-IN", "a1cc760e2498219ddce9f1047f957eb3bf36813be97c15450c05722bfe2f9362d6849dbac417a684b94f26b6ef45840c0815182859c3c262cf3aecf6ce337fd9" },
                { "he", "0a40cf9a74266c9e3d322e1973bb2934ad96e1558b1c745be2d16bedd192080e6ece844e572d0a2c9ce2a006011c916552a1f5d1da33a9b34ab4277d8469d295" },
                { "hi-IN", "1d50dae5b5f4532167610c8df0bb66989473cf8b47a246ca1b6e0e3821d9a8240136104009265121bfc331ecff15d58df49da812141e5de58bd2cf6f81aa74fd" },
                { "hr", "996f02ef6f0077df64290dba21a8f96a369ee01128ce70ec0b4e22922bfb3c74ea4d27d1ebee50e22e48c419549aa7a80ae3d537f7917fcc685fbe6edd00454c" },
                { "hsb", "c9919beb894f4c657f16d3382dbb71526a395ffa82ca0a63be8239651ae42f83828a1972bfbc4c99869b6f49e5f3bf1224958af0d54c3dc091e116ef5d6774ab" },
                { "hu", "efc5600abf832f797cc8383e907938793d19f5324de6d9bac39bdfc49731a47c6358d49814b7c160b32c7f1ffe24b37b58a23e62680f8013c056c197f7358263" },
                { "hy-AM", "efbc65c6f3bb217ff9c6eebb7c3150559e69caa8662009e0a152c349e2bb8970b371cde89473d918c63238927a9b4c2fdbd9c74602148aa20a738239af7df28d" },
                { "ia", "c4da804741795097efc85190ebc7e2be5dcf4d2e958c10aa6014f820833f50ceedcd929688540d7d5966016ef37f958e144eee3f6c3c120fbcdf41ee5b6120cd" },
                { "id", "8c80fab99ad3208f8f196aff9340dbc18659edb7df50acfe94c8e5c0113625a02ff896952cd493536e61279628c63e0bd78666f2387fa370e1d03443c0abdc3c" },
                { "is", "7951d24a75c397038751e6ef56fe771a48cb17afbf86a4e9b98f57692e8d2eefd8de920322381be9d65422873105cfe06b68b2807ab78c7f8affb1424e58fbaf" },
                { "it", "3acae79823e15c25921ba0e3bb8b10c94e149c1bcf6fc67c08f401e268d0e7369f5de99efd38dce6c983eb68f6b024db88a4f27764f3908113f347a60c4c4e75" },
                { "ja", "702a47ea576570b85d2720e06f81af0bf6f439d24bd8c159ca1f40d43cdde0b5bcca25bca5deb84b81bc8b733126cc9df38be6dec9e890cb9ad4d4215f97389b" },
                { "ka", "e82842427d6f11dfe52090a17899516b9ad4ae508e6e694307838ad235a46bece0005286a5c14767b2fb1e7432e47d1bb8d5c150b5e500cc863da472782e80df" },
                { "kab", "d798f0b6ea1c93a1390bcfd77ad72c3a99708d8b43454a663abae17da01557e2aad1df25ac398d4735985e8423ec554c89e13e7f2c14cc73680afd5705216a63" },
                { "kk", "7a9b8072518f8d319c54326a09f22ea966e6e6868dd217205b7a477d6c1b88d180855d37264e6089b15dc2588b9f4445fc8d5e7141791ff684654b5e9f8ac5d1" },
                { "km", "65dbe8be4b67128fd65692cee91f4af5eafa590f55a087bb1dad8bd004701ad3e9bb77c28e33b7d7310e606ccceb6a23ee139ae0ef19940aff9a7f310efd8705" },
                { "kn", "0ea53bcb0f8f63ece1efccd4120054f2cbf55c8e79deb500508321148c8ff8742b3e1b0da7434c966a157256a7f2c287af8362fd8706a462e89fd366494145a4" },
                { "ko", "317cfd8020bfc4f2d7b47190a2c41cec62bfc3378c8c84ef2ab79b7a606dfe06adfe73fa7f060dca4a9f4bdefabbb4f3cfa711c475bc0e68882bf51ee9101ad5" },
                { "lij", "3332dca617382871944b7495f63c8e3ba8ed0fc5d9dc38f978dc4c55c5b91ee55a27efb98916bab33b192c4782c99587ea193053a0cb184c25977a54f5e38180" },
                { "lt", "bdb35a9be6e20aec316135b2ac364dd8fd7b4e98a4830bd2e8f4575c014ff8fa5d33b52796a51c7049fcf878987571c45be98884a2c3bc0717cf1812eedb870f" },
                { "lv", "c4219974d2e2237b504d25b211d2c543f880f3d4a7e8ed051660bb7f407aa5211514d483b7a86e6ef68abacbbfe111717b250ea3a2df4b042bfa5fe856ad568d" },
                { "mk", "74ceedb155e828493d76c1fc4c8b93775278b3ae3f40fb182e62cd860fb0f6150f5b3b437d5bbfc768090dac8e55e747e79a5744793d2b7cc280b8cce3917dd4" },
                { "mr", "4463842c4f41a562f8203645c1e99dba3a2bf5eb366037789251a00e0e15dadde1bfe0cdd56c0dda6c3255173b7fc32008b6d0e61f40bca906bbf9324fedd8d0" },
                { "ms", "947b7a79cd5c20fd09c919b750750f47d5db33e2c081a5f7eae812e8e294484da6ce9912fe8cb66f3fedf8305a895f2e184ebffc5b8889ec732a00ba6f7b78a0" },
                { "my", "df5073fcd42eced4100da2d3d6ce0406a1441593063a0373327b51a8fe7174ff243b3873ce28c8f92107e77259bebad3f19d0329e5d5e4670997d2df28aa06f5" },
                { "nb-NO", "0a58321af054c68dcda100ac0dae2501a66fde0ee0211e655b80714f26b6daf0cee0bf2f32e89ef31ea3bbcdf4ee9ab12b6e2784c28f9f5a3cf219d6acf1452c" },
                { "ne-NP", "e06e59e78eafca48dc0ccb6d5e1f623700080dee0b52045937fbfff73b59d7c684acbf98d48b45adfad78f7ad6e4247af772ad390cce6d6765fcd626b89af783" },
                { "nl", "ec607f6417ed7de5ce648b30b2d137e48f90c71fb74cc1bc2064e6174dcba02fbd0528c48792f55f340314400702ba221400f6dbdd5644586c1855d40129d108" },
                { "nn-NO", "7358420aeafa0196a1c7b969401a0888a8f631597256a845d76840168bd57bf36fcd3e8485c94c991b3f1f0a2cc22376e612d4a9de9a5c1ae5e1791ede0b3b2d" },
                { "oc", "ea02eb871f545d4c1d9d2e1b9d23ee2b12988fa77979cd96545fe37a86d5d3338b9e262f88f2304782141e7613ed1f8ebf79c6c57b67fa9bbd5ad44cb4b5cfe6" },
                { "pa-IN", "2736620e8322cab8daeae887344c7bd15c426974adf7ca6eb7904613eeb3aec7c666e1992b21990c9d34467bc6394e090397156ef3edbc40162f8bb2f19f9090" },
                { "pl", "4eb61587ccb1865f9e074e0a07c3392452d0d2cb3a480256d4cc49b2cac2381df8c3a0062ea67055beaac393d1533f1b09cc6b79fcdb76d561ad17ad40db33cf" },
                { "pt-BR", "2c855a665699f31cb9e473ca890c45959f12e03ef0b1b5d856eac3d7e1fbfe79e0246f1049ea8090e859dc389ec9ebf5e22240a534ea4f419f24cd61e80192ec" },
                { "pt-PT", "43dad89a2745f957832ccd0ffd223d6fe868b24c0b5b4b997c2a0cafac7b49b5d730ab00c127d3a0e047ffce6bb570faa8bfe0e72229b6be7fcd58e756bc00fc" },
                { "rm", "75e371ef292fc31fc9efab92c194d63874f09b6af999346a36cb2cd16377d0298bbfcd1589836a71fcc95d6dae069b41ca46a7c54417b373687ecc636778bd05" },
                { "ro", "e5f196a0a4ee2d0fa4806957d167dafd380db32b62760a3b4515bd630d836bb8e960889a7671bddee1a325785d91373e9287808f7ed3872e25fe60ec125a1071" },
                { "ru", "901add993c8a91ae983e4df8f7ca39b76cfa2bf9c37341c62cf2c4c1f05b9e686a3a4683e0ee335260dc6f440daaf4a91a08662203ff906ea17cb17ce870bc1c" },
                { "sco", "192b445a1dbd8a03c76353cdef14aedc1676e22c650243443b469cf51939eb5deecfa938a09823c997860a3ded75ef0f7ae13908946af59f9a150b2306e07d65" },
                { "si", "674a6f2d6dba60107bad9e0a39cf9c77bc26cae95adf8849ebf534e9a3e2208631f3db810140230cf493a8df7200268d5121d604d3b5ed7f7ebc9f86048352e1" },
                { "sk", "4380ba1bd25b96de56faf58578074dc5d82d1ee306e191cf6d156f965dbcf24a967962517fa72b34b6101d7132a3c26fb16a8349e6a62f62714e5d4351ddaa13" },
                { "sl", "196112585480e586bc5ed21b146cb9d36e99e2e26a99fd990f8dfc12861ca48948bdb7090b2c4f196cd624b469de65460cd1a472c14df4780c58882831f4f215" },
                { "son", "d0a2d5b823111442f3a69cbe9ddecb1a253f832d8fc9e6384c47d649a0c21e565a102d3a48f4652547b001045a5c33be5e1f988844cc3753b5655a1df7236d6b" },
                { "sq", "816535de5aab6d17ed0f776cc1b86f3d36707451872c953bb2286f1f46f6980b23bb832f88a0977681dcb3c07883ebfc432ea68853909f6e771fd48fced150d3" },
                { "sr", "cf344f2464f6723fee075c312d1e88c77e475ba0a0901aefe32fc38563a13024004c3dfc44d0acffbbf6b0d68ef243ca9af4115a4b7f1e7c906a376931675629" },
                { "sv-SE", "c20aa7fa150cac93fc59b2a07ade0a5a514e711cee7b9706241c03659cdc01984e13a42780629e21b2f5f8f2c5a811b9ea3427d82fc6873c118c62c3eec8fe23" },
                { "szl", "45d7d23a8dfac8cfe0272cfa0973891a9ad49946806e5f171466f0a8e2969bc003eab793a218e6f1ff7bf41e149b19d4dfec1d47046c49f2e81fd8f12f886437" },
                { "ta", "802cd094aa3247bbc88ab640942aaebc6074055d4f5e0271b49ca74e8c19bf313926ace621923bd5857ef9dc78c8bcc9e999207a184df2dadf80bde143326bec" },
                { "te", "9427ca337c994b3799441da5adc53cfe2f347d97e56e3a344ba8eaa8fbb2bcb8b5be65fd0908027f25cca8346a50f93a6ed7889851805165fa6fb1e4fc21d2ed" },
                { "th", "3ae534c1dd0a045daa1147115c200e060ddf65638db2a4b494b33d4382a5606fa53c097989248e9be7fb21f4ec1bcdb644596ede8b54e63b218e2d63da2db797" },
                { "tl", "09a7c8efeb3f5a128eea082fb7c63a69d4716a9ea8478bc8592f29558a9dc05eacddebc9ced52eb4f5a2cab854e7126cc0cb7797c87b9fab421031530e688ad2" },
                { "tr", "550729b18ae7c01f3dcab74112b305b5315c6dc8794949303c89f99081546ed823c0951defc84a550c34ee7a38a563a150afa1a09f09cc4368a9c6a2f90794eb" },
                { "trs", "0afaf8e338f9e7f24ee6eab0183c3852ecc7a047584bc13c3c70d4bd948b6806df132945139281efd8a6687d333f64923d16c8d433074c762a8a696b05b8cfea" },
                { "uk", "028f0031dcdad809b72105fddfcd694d5a263f0accb8b40e28979e5c9d7af0b151d6678d701153e49f3fe35c219160f24ca8fe3ff7b12c79b2ed3cc7c6fbea86" },
                { "ur", "d69c2660e1b2d60b4ee95df267127215e0d8d3e2440239c2a015ff377bb6c4c16196f9e1f38725ada42595d4ca329996074f25073af6cb8f27b71575eb470ebf" },
                { "uz", "025030b99eb44eeda8e2096410ebe2c4297cb850d77b2924f871a22f3ad124086392afd8c8a84b3d29495d01edb907455873f97841c92927d659e016f04a00ef" },
                { "vi", "865c99efa6e14d5f733541d5138cbdc91fbd84063729e38903d95ae04d567400aceb66cb23e4bd23082554a7183355553c2803a37859d1c8f6036d3fb57dd0f9" },
                { "xh", "6cb8d157e59981aeffa1715b5e8f138e43aae582e2153e0a450cf1054d32a46a32f10911b854e1c7193653f7782810031bdc85c8310af2a243b346b4f00ff596" },
                { "zh-CN", "1a12dce9984f538c723009e38e95cb781c8d084a3152d4714947d087a6c0c4df8733081a6505097d6bb7d6e9575e8504cff18f4ff6757695527470a84daf8828" },
                { "zh-TW", "043e81188dfd97c7eff53260c71c9eb2faa7fc9b5a741df9e7cdbc3abfda30f42391ade348614e027552d2a3820b4b21b94a87dd1de107218c178229d4e0a6e9" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "7e873bb6b412ded8b8752bfef84165e701934422bb345721e4edc818ef0e6bf6c8da587a380b6421a2884a893975e4331673ec5a64c96bbb24935cba5fb645da" },
                { "af", "bbb7478c60db3615291af2c8969734c15d0ccea2e632490a7b7bb5c1a3947233114b1f86bf4f13f9e0dfc969bc02a5a6070003c1492dae6ae0e7db1c00de6690" },
                { "an", "460c0576fc0b077aa49185f407e999b26ee20115abfcbc7982ba0cad3ba375a878910965d898caf196158c0a4b46986eaa652193433feb5a16786f7804aa2e65" },
                { "ar", "4839c1649bd60eea440226ec6ffc52f4196aca122da283f57c799eeed140ec6e325828e7f92b04a04cb076d76c12b0511eb81071d994ffa9ada4d8bf9c27af50" },
                { "ast", "6c0dc2837941ce927184a8bae3b4b1a7f51cd8d8fda87f87ff9ddb215e137ad782ec41f85736cfc8f691f37616c1c81ae61eb912b9897832bec1e7824935cda1" },
                { "az", "2e46ce7c6176644a0f77d0dac8101748b2ba5e3337074acd5df08fa0f04f9b527de0149b959144ae7924ac2a158da08f060b6e6a959ad9f5246484073c8f2a2a" },
                { "be", "aa896c4924e61366d17b684e690b787da887e2f7406ea6088b2a56f26d10f47253cc97bfa2809694a42f4501cb425fca9d29f899ca4f4951b46ed5e2e307cec7" },
                { "bg", "84428b6fd2235ecf004f11d2fdfec1cfdec231c877e662f0f96c725ac7e412ff5df1778801d01a6efc265724ce517f7bd1df8b4ff542afa272e35f2f46c297c0" },
                { "bn", "a2210656d0eb2b69f750cdd7750e18304f312d4621e33989710531ba8d8afb79710d5abf7a55751b14ec6d7c6884cae75107594e03a6e0c03bea4af0b12ef8f1" },
                { "br", "7b57d99a2fffe5fd86ac163117b127dfaa2596890704a338790c048bccf3b863bf8ed4908165e05ef9142426050a44c488e12c0a0d2664442a129651f9e24e16" },
                { "bs", "f9f054a164ef7c7214caef0ec7b305f56c7a40d8d7a8d6dd44e7f2f612ff155e1702d856fdb73d51ba6c7c16440a10f670309cda70a8bbfba4d32ac30728cf28" },
                { "ca", "af49acf38a3f0558de0b2ee032db8a7fc2f25860212030096f6784233e1e13d3c93a332cfd1e1dd5b364409d9a6bdd3d0cc8e25f1f8c58c3f3583d1e2b693130" },
                { "cak", "59b572f0d3df8898b3e4002b2feda32fb3fb431d7ae02d9ae6f808d1a0ab5831070ac9f57448fecb2d211db68d488e90f667f09fdf3375c4c142665df102058f" },
                { "cs", "3018b86332f1590bb642409f6415b1b7d7a483030826ef0e892cd16cc7732eaa1bf14c6bafbed4ca679e1f45a24b86c402ccc651da8196ee280eb15e9fb17ed8" },
                { "cy", "60f42e8f3d1bb27bc989a7ab966ee7c1e57d80b9e016bbc6aaba4296d9c48e828f84cab60d0a42f49729063c39011a62d830dc7ee4d7a1943e8be11f528a297d" },
                { "da", "6743accb8104bea6fa7a0df2a016bbf57b56defa738ba8b55b07e9f841873f5991845d8106bf3fe916b6ce19b93451c8258aa7fbd6f5899e734ff961a454b1f2" },
                { "de", "5b94a59df3a52bac81b0c39664f19f2af8ff04ed66ba90f081c4ce70ac6167aace1ea0e356eb3f8f2644350ca220f48cb5db7cf074514be9ae5a670b43f7d8a7" },
                { "dsb", "44bca32fc6a36947a356c9d2b994192a97581c0383001b9e4a4a4587460bce7a439721e535db709bc02ef5622844dd1d23c7cd8399b0fbc347d27beecb7b57a0" },
                { "el", "37fc5611a00a36f637597f557e2fa509283e90605f0f1b907d567b21beb8c34e7c2d5932dd35f136e8503dc889d1e48852889aa2a7307eedd5b173bdb68521b2" },
                { "en-CA", "8be85924dadc7f5bee5eab9f0e5f059c6b76ac846964c851d3bca7ca6c39cbb32bec932421f8fe88793ba5fe5923ac2c55e9023af920f1fc5abb86ff6fca0e3b" },
                { "en-GB", "bde285b63ba592f315c9356f50c9a107d954bf861a14fd5545883855645e188be8e96ce5d0b8539c18f80033af900349189d3e4e1b36ccccc02ddafff7a2c539" },
                { "en-US", "50110a411b876f43f7869244c3de248ec8a4adca570f7ea176076b94612f2cbe747f9e89f597ac520bc658c2fa0676efcf90660e902902f9a934b03f0da1922b" },
                { "eo", "aa5de1b52f93aa3dcf501435611ae6444fcd186e409ae313986f439d2ae0866bc52329cb25606e090e79cde28642a2ad76a5cb6d65c596fabd8da3cc6f29e2fe" },
                { "es-AR", "5dbcf33e86787ca7ec066c5993992140b73ca10dec00a487ea7e46bdb78524550f1fbaa9dd7de14b62ed1dc6edd43cfab02680f4f5c912ed04d0464cb5d543a1" },
                { "es-CL", "d5da1b8a0e0a86dd05b484e535eca134fca857a9553f83a0296fb8079525618619f4a6a45c24e7df59f924489cf7a9183ac281749ea815851f3cc5b7809db857" },
                { "es-ES", "a7f8acc3e5e3e1e7cd8ed17262494ec99cb5e4776ec8b946ae6de22d7559500b92847a7d153fd3688fcd517a534fb25e0eaed99c8ce2352dbcd776b98052b323" },
                { "es-MX", "8f9171351edbeee9c67171f423c47d8106315ddb33748fdea7949ce7e883e8ee4cc6c82ac158954f9ab94df8a2f988aa9633f02868b724bc67e7f37deac62da0" },
                { "et", "c78edab20dabf606f60ff65fab654c2d728de8821f07103a9e600e73fa94cd87f42ee4c729575e7c3c8642ee2d1ea7ecfe61221cf41102a8830d4f8fc9011178" },
                { "eu", "f91eb8580211ff5bb816e37d6c5872e9407b252747007be455a6551e4aafaae755452aa24cd2272c9a12e62189a48459612b3c453819de12645a988264831fb8" },
                { "fa", "e042becd31599c0a23ca05387207e03ff264ad106fb8163d7b282a87eb3994b00f56258b137efde09cf73b0fb48e4366f5469b479800b35616a240534b0b0020" },
                { "ff", "e44d2434b25dd76f1c08021379f06b3d0a66d2335917a0a44b8b6e4f53a1a95fabb930eb1a936ec3f9a83d3a39bc074eed6f07d0b3724d30b6bf90f74dc6c591" },
                { "fi", "69190e09a916c8cf184efb2ca92f2772daf9b7291ae1c39a2ab2a36e2a139a4d81904eec1ff26fb95a2ce1a31074d9921a7b2f3d4f66bf900e47f7d63195c626" },
                { "fr", "3e5534ff55111a0171df56ccca7e4757bc6905f2ab7427e0046cb8bfb526a2e1dbd90b2919b6affbb710b684a77ee592d98395cf938e8c900ceb435531ecfdc1" },
                { "fy-NL", "48517f22a3eed128e9514afbce1bdb394848d73a7070523946e4003af2166064f510427370295cc02148a846242eaefc9ec92f77326382802462dafd1ff316a0" },
                { "ga-IE", "04ad9dc236da8dd18e3a0058c175f112c504410b3d75182be39fd2fb34d92eccd034d834e5df10c04840790cd44c7163a7e763c2c7478cdbfd60e0d1c9f5b3f1" },
                { "gd", "30e39867b5abbc824b3385adfbf5345d16eb40e55ad12c834424e862934667352893ae33f06b6d954c2bda4899d24c2f22dc21eff4782c11003536878f6969f0" },
                { "gl", "90b721a288ad1862b315c951f87be83c6028f4047f842d975bce25034ed8be086838a52dfc01fd826414d615e78d874adfa7b54c0bb1912eafac7f1e20591a20" },
                { "gn", "0a328652f9913ee3ed88864dfd5a3d38786738a2559470af8331a5d21b781225c4a97c722fe13e76e1cd5f737e39fe334358a63f8fc978c525fd203ac4693d73" },
                { "gu-IN", "d5f4117290255af1f597b9e656fe278f150619471bb96d3a6a42b50057f60119e02904583995495b6c44fd16a07aac03499c6bc61b3533459a141794b005d32a" },
                { "he", "a3470918df8fb10c5ef33efc6d0ebfd2d2ace997aa98d037c1c9cb847793fcd77d4371dbb5a1dfcaab210cf48e3bd596a2db03d572267e25f801877520c336e7" },
                { "hi-IN", "9df8700b54ebadfa56137d5a56de2d8021fedc5eb7b7357e9484431b18b004272b490602a7a7415af6fd311d7880676142cd7028b79db5d40f18b05de2911f64" },
                { "hr", "a21ce7a654399cfef9e95286978bca0b8a17ab91ad958c43505c380964c90d86855be6ec059f52b38749cc04c137fdfc17ac352eeaf8bae6d7125c1509cfa3a4" },
                { "hsb", "b09cc9323ade399c31a071833bae8dbf40d838df775beb3ea5a0168b8d6b83d29f8616dbab425b7ef51fa513899095131b5503acdcf02fbaf77604ac7e49bf24" },
                { "hu", "b65638c262f5a08b2df72bdc9b2660a8f5edc288972bb6c349c2123bb1e01750cf1e6e1a0e8562b21ae35789d9028321491a69bd96bc8b2ee4983faa3dae65ae" },
                { "hy-AM", "6dc9f8d955f91f55f1cd6b949810296ca750a1770547d6e9365c464cdbabed5eace0ebf6282af3e396961a448727af9fdea01b1796b44fda6ad1c6c2bb83c530" },
                { "ia", "424a4eef42e740c848e9e33cad04464f68334f6223f28107a61489aaf0b013375c8167aac44c09262237b7c80fff8afe321768bca0ce33bce25db6f9b652c12b" },
                { "id", "98f428af903febe2b3ca05a9b332e9ae8f072596508f2be31b71beef441c33dc309d1580c36a5461ce4e8f251b8274600720e60007d04119e724671211e81abe" },
                { "is", "d8b954809d90859267f73935919389fe9320d9f69961d0d02b09bd95836930c9b5ecddeb1e8ea83fe9926a5840a3fe1510fcf4553363faee150a1ea711879efb" },
                { "it", "aecdac4f78e7e1727c0a230c604982405b13584e9b2eeea18fbfc99b91ea14e801c09b073c5140c42458ba2a8c35b0c5ad989f194da9a7daa0adfc6d04109869" },
                { "ja", "f6029aea6196c938156d6427e6d2abae14d20ab397d423fb7bd016b254591c836cd4f07d6a89d646804e1937ebcb4a82081006e2f7747544c239886fbeefa227" },
                { "ka", "e68bc675a7309306aa4ae4ed6c14953a50a4f13eb185ec77543367df341a38aa7308632963f9aa3f8426d5205ac02089d2f056b8304f670b69b11df2d6587424" },
                { "kab", "74ebb115f7aaea3142e865ee19bb5c0feb4df008d657c41cc4222dfdbed091e1485f4921ca91fd3fa3b81faecc21f33a5cf706272736afc9799e4b1df5858bf5" },
                { "kk", "5994b01f9c9cd7e3190ef8d5ccd8240cdd304a13475224c38f84340ea5cd63b1dde29f75a55287413d12bbe6e422104cbab54d2b795b20d786a59fb364832a88" },
                { "km", "b8a30f393493ba6cc925098191c475b7147c71add21c5defda7756dc1d1f1500a5c123176d44e2cd8a5f93bb91e1a669bba64c3fef4062162750b64581d394ec" },
                { "kn", "8dfafdc62ae84dcf493ec02bb82249fa09e0f058d421429d6fd900e0b84f18470bfecdffc3eb9d743723e469454b032df02bf67124625daa3c1ff0b17e8d5bf1" },
                { "ko", "468b3cf0faac44d573745487f639392aba8380ed18d5c0304cb51f2603d5a9af5fb35709cadb6e0e4da8457459041221bd1258586c6317b230ad8d69d09d5568" },
                { "lij", "281116eefe5ee9f40565d436495373e107254c21bdec435884f4ffc8b4e4d8c5e642d8f529ebbdc1a9aad858674f664b3d9910212c839a4b3ad577c1344f7512" },
                { "lt", "492a8c12034eecd00a36d50f0c96b7b218a0481eee33f42cc0ec48e65020b49abfece104ad8ecbb9ef5f366b7ac052c9108eca423536d6442bbf074208eedafa" },
                { "lv", "e919d6e0536c26a4387be115f440c8a4580aa973b2dc1f9148f96688614ea42c5576487607346f4c4aced06b6a20477146501774d4f21f654551b111095c6fe8" },
                { "mk", "812c517452bd7e73461d6c8ebe8672508660700135719931cd403e90596e665e74217716b60fddfb2a9c05588d6795e88451f233aa8aad3fbd58b482259b14b9" },
                { "mr", "52973b2a78b748cb6c5db20282bee983b17a1a423d65357826d85dba172a42bba3f2774908883f1bd9a350699ba3482a8dd7c3b8696127fee64898398f27838b" },
                { "ms", "c0c61ca5744c6fa6ef72b9d1d24b386d48b2300e02c3bf4ac514030e2536e37ad9da687e502ba007f13ea3cc06697384141f0d3e2c075816da40cccda332e401" },
                { "my", "561414d7c9aabd7337160b319556f19eacc87c4e0af7ba19edb54534c52e64c97e9107e3fefcedb94cf63db9ba8458ca162c52c2c5ee5630546abcc081a865b6" },
                { "nb-NO", "d5791306ff69c52e6ce6b158fc1525574601170164e81dc5b88371dade184cfd3df78c83d6dbe24bc2c8e2b0b241e7f545e404d3b82216876c62603c926870f7" },
                { "ne-NP", "80bcc96e202432638a16f24ab19da6ba8152993ebd1f544752e8c1ffc4321010715aaccafd11abcf202c36e99aa73b1f0046e8ef95f631173bbd0e16a9b920b5" },
                { "nl", "1c6e3a547026b1df3d00350fd0b094a8edf8378c58ecd03322b3db13e114d9e258e91c1ce1adcd9fa369bf373cc10942890beac8f838ce94257450d79a77618c" },
                { "nn-NO", "aad40f41a07880ea4be9429cbe351a114e0d5fe4b5fa96945db90c263d7835abe1b22263bf00486b8583090d1518595525fa8f3e4ebeaee9264ce95e58eda9e2" },
                { "oc", "7443a6c1fc371ce4aeb77e80c826d3f58095bcbd5509b0b6a5cfe0576209ba9a0e2e890213ffb4fece6045a2377fc30e8e3df78d5d31f7bd3311b7375194c637" },
                { "pa-IN", "3c8926f3f0e0d64ef62a8d2686e9a6ae74ac014aba3192b0f1e3003c6bee1dfe82c95ad9b241d9e29639c883d113f98ed5ef0fbdbbed11d7da595a99b32e85d5" },
                { "pl", "4b2b19250e594f8c8d638b7edcb965f695ce30d9e941a8d32a3439a383fb55f5fa22470ed4ad41a4c9e5ae369b2e3724acf732b755b8945fd1e53fa403de81e0" },
                { "pt-BR", "2ee5acb41af7a1e73ad495074104bdf0830454503a6a1c57aa9cb50e708b01f6e437e13a89efaa483cab0f8cc8c0fb24a814128e0dc960a5469deff5525a0a00" },
                { "pt-PT", "c516b3c743eb221bdb8dd05c80c7f8a9ad540ca17e7583902db630b46631462da208a244535fcceaf81e2351ff82bfe773d2d369a133d6fca4876a309793b1db" },
                { "rm", "e6988554431d9ba00d982137ac40824ee196ef4757b483a993c2ca0133e0ba627e0315c33a95059cf0645880d5cc2cc1a34bab8f6198aab296776efa156f473a" },
                { "ro", "60ecabcfe9ca4e561d151221b1eb21e5bd8ff3e7fb79d410c15447565967307e4ed65d192e582dc0400adf0ca2f985eab3ed725ee9529018f0033e7aba7bf23d" },
                { "ru", "07126a7e0cef5bf755e237e9600ecb3dce3b4b4d81bf93ef9968d45fd7dd816ec58e4ea94cd68ce8ce7b5ca5bc5eb6bdd5cba1e092bc85a97ab3bed3a32421e0" },
                { "sco", "44da567ee425ab3d3e16f3e1072eec1a8a19a7b00cde48b9da92d4c8a985c01ecf9ba902f61286f90c45e7db18310b8f489b307e3323fcd81fcac4396eb22bd6" },
                { "si", "4c77029410e573da5f0df6c47cc6fc7f13e09c7d2db8262b3a4168abc42c7809c64991a3bed96d2923e7f3fdf3b6cfd3adbae7e4318000743cbbfd210a210481" },
                { "sk", "3916890d61fd1dbb062b2780b2922492d5034bdd716f2d648c2aa57ff140e19379db62f305231b3ecd7ae98e16ee42b0fdd0fd39e08e59568f2ed85eb448a294" },
                { "sl", "09b6d7ca510b7d2708604cc147e949210425849946e1f089eb1133930135c2b8a5d2b82864c6f018679750899d9c48bb6986bc899d546c987313e60fac5e9528" },
                { "son", "ad078fd3616a0600e05122da6a861ed28d2403f85798cfcad3733e32638e084ff57934b363bb7ddc1780aec36d9db5bfbd0d729a7dd60040fc4fbfb3a1ad389c" },
                { "sq", "b4db774c2f0fca69fd5345dd9c9da112e838324705e72cfabf8508553a19338e27bf4012f256d08e967aae0421e2ae286e36ff2aa36bc963f71ef6efd748d057" },
                { "sr", "7ca88d1ab7d1af859b6b8093fbf98374b27d5fd314bc6a389d90d18405a701e20d12b1b4ea6921e7618e36c57805a84370be21e545f2bd5b70a34eba3c44a617" },
                { "sv-SE", "cec7ca71a88331e3eace4ba06de7d01a3f4c5f00a1ace819942deab329d2efbb9dfbcdac015f9a6d4279484dd017fe524d764bd3b598c078f6ff93bd234ea7dc" },
                { "szl", "3422224066b71f4075e9460496398efb0ccd31be2bfa55bd57ac5c66b7a1486cf921a1c53c4e1e58242ca7ecc84115c5bfebef8a030f5fce2a3afc9d1c7b343e" },
                { "ta", "f7f7b4759ca342fdc345279f0d7aad02d0ddea6aae7aa40d2df68bb32f9004fbc21305c76e54f92423eac6b8b80ecda6b588c40da7c2fdcbdb8f40f3fa3ab994" },
                { "te", "2b2c43df1540af640bc88c11886ded522b4b3c050d2260f32918efd4622b8371cbf469e8d5bd384f84526aa8ab24d826900420d1acefa598a15bd6251caf64ce" },
                { "th", "895e7a7f41d23595e0da6d1f6fb1fd4cac1778de30297c87c9a7c9e94b7645e88eefbfbae8a5d5eeab7d1a613437b2bc634b37d5bad029d4385c5120edc0c5a0" },
                { "tl", "5a22dab1bbcda160bc0397f63a816bc60e38cdc1fd6dcd3db2101848ad2ace5556c1f324a0021c5ea509e050a90ec1f1d3ed39210f6309231eddbd4957444360" },
                { "tr", "4e6894685ac4bebde6252bffedb5abdd8d7dff7db157533c90fa279d376c1268f8354cb0a5323392f853a09ed875245e733b8f43a048bf343e305009de780e47" },
                { "trs", "72a8aeb69734c7a539618421168504d0daa0b0e94e7e4800334062d22c17106380c99ccb1103105e705d08b16e43f68280417ad828c484cf8884be251778bccb" },
                { "uk", "6237b74ed41f0bfefa90ec755ed0000b3d66d8668485a735a097db65c0b9929f7fa99f882b70a2f6d35476034f36fbc57d566e1042d12ae2db78a0de8dd116e5" },
                { "ur", "9a5d1100455b9b3fecd1496690aec73e9d6ee07bcef8acaedf78cc9f1857da37982e09298dfdba4cd57aad8bd24a931c3c6ce6aa90fc1d8b6a392be57d44c18d" },
                { "uz", "55278e0de283faa95e1e2219a4b6a03671fd746d123d3c1b9b32149888e0bfa5b66d7a378f4f7f4342f4be601fd37590632e4d4fa16407acb3288063bdff205b" },
                { "vi", "609c9290a138338b94fde6c37bfdb00d73f8e774957de75a05bc72cd29fe374d9224a182c8840d662a5029a370d8a3f7d71ab332911c7eea9e7d64fa82226623" },
                { "xh", "e41b4ff89b14ff4f48d4358f85bbc2ddcd3f62f3756279928cb81d14a876a0c6537b3d1d9771dcf4dcbada9310d2bf32b230a524887eeee795d45e534fd06c38" },
                { "zh-CN", "53d833779866aa2834fbf30adfdbf52a10e722f1ca1c557db3cdf3db473cd98ca0704eb891ff1951781233932b344744c62d32a2a6241a3ab2323d432a50ece6" },
                { "zh-TW", "d1f6b4c0dfcbd34485aab30da9b5e40660c37899ce0938a444f8fc22a755e2208baf6361f8b26df8ff2ee8d3a82a88016f8bb2aba36284ef8aa69ce5d9cc3f48" }
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
            const string knownVersion = "106.0.3";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
