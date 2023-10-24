﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net.Http;
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
            // https://ftp.mozilla.org/pub/firefox/releases/119.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4f4ab34154caae4f2449447de47c9c37588b9afe870571db7062629cb55c5f865b8d3c30ce7a08a0d7ccea4a42728b74e4ee01525dab46035bbac97df54195c6" },
                { "af", "55e622a27182cc2100e3405a408f760190241ce154e78d840547ef559bcb07fbe481662845e1671caf7fc445726d71228b97b70141640160866054bdd78f8141" },
                { "an", "dbc0b939587f7703920bdcd3dbc47703767394056028f32367448f1623715a50ff0e39d1e155d15a4a88822d41eac59e8aea0a208606d4d3ba05aa4c1a8ac599" },
                { "ar", "58ed995cace78ddd20ce42b1364ef9e74c083805b76255e1bed5ad8d0218ee441cd5f0df21c98b49e08dbb491e5fd346e4288c6f3bd3763a3957d7ce306a042d" },
                { "ast", "26ff326d5fa5f5db80c42656d9fde82cbdfaa593deabfd5272fc754854d71f833054bccfa70dc575d952f1493e8c9f0b850cc149993e86e9d28e5fa534464a1d" },
                { "az", "ed6d9acac346d7d0bf2d3638b6deded0b8e37d9445970145b99a2c031533e52dbc25be21ba15369d45e2658e838690129934c2fd3c1f65db08579b19dc5ce994" },
                { "be", "170713017ff5a3b125b15697fb656969c3d1324fb731df67bce3a4d356ac440b10815a3f00f95ad0943ebadd4fb63be336db9a51172267ad911912a7655f39e3" },
                { "bg", "19ff1f5a8ef1e6e392c48575eb1fb70dbcc1ae4b6a073bdcf2349beee5ce8608ecf502ac396b62a68cf97323fb7626e96887ac6d1e0be0143ce38a334e5f2600" },
                { "bn", "aa0de58c69cdfecc8ebc4392af9b63cf43a8573591d505a235212751ee20bb1fda77badef00f93a92a61c94d15f488a101f99d8bfce7badae1d92fc77cb477f7" },
                { "br", "ecb4751706db8ef02d4bc89aae5118a4dff63e0d339f75898b2b69f77a37f95b381478fc7964cd39fa83cdb3a96a23f747445f9a95d23d7fba5d5ce806e9741c" },
                { "bs", "76d2f460383b0a446924cf6de9a6c550a08ec741d91e1bd58a8fb7ecbc809a0138c2258bbcc8ace271f72ab8e7ece60e05aaa4855ff6d603aff39c07f8654946" },
                { "ca", "09c42d9cf4cf87666a0afc9e13b11ad00048ca448400eb7b3cea9e0176acd97a0c633bc738b73ffaf88bb705b0fec01fbb624c291188ae81a49cb7b7d9dad25f" },
                { "cak", "dd557d7d311707385894ee9c639b074e9df9db348239a23b137c5e8dd5ccbbfd645f8bddba07b0b02a2c727421103fc8be7087148e46fcf5a64ba1a38f6624c4" },
                { "cs", "d0c7e7b6b14989a945a6e9bb9ac3a15fc92ab11a788e2becbe4e1c0111d0ac84dc8d0cca7a6fc84ab13000746df96bb5bbc05d821c953dbeb82e9ec1720f6a0e" },
                { "cy", "1b257ec24ecfc005377bcc882d92e9a5a759105c61d7ae03b32cd46e20d3e30d221a668cbefdfd042f0904c3abee9f247aa0821d8b8778675b97b99e829997ca" },
                { "da", "0ee9a2d1d713711560887d145f042d39099ce3dea254c355b1e95ef56f26234a8f7128945df7b145aa6c705bc08eaf48cb4eb2b76e6d501ea0b5e208fb6e5c2d" },
                { "de", "b4b92dc220efdd0ae4fe307ba0b44a96c9b490e815668809d3cd23bceba3148058a87ac49a1bf27b376073e1afdea4c6460679191a96107e1474957d2e4c02b7" },
                { "dsb", "0db86b7c952451a3f2c0c4f9092cbc088792c82c12cac8574f86bf7bee4cb17aca8dd6138a28c2e4ed686ac1f113dc724b5cce4c75f623a36f0857e10fd5e0b6" },
                { "el", "81bc530142c60a456bc3ce0daf19b2e9f20d3e12dcd7e384e4a223d29a68928a2925d92380c8822255644bb1672a4c9499323bb528c85b5b5b60cd92a1a2f9cf" },
                { "en-CA", "0d6a348759d48d662544e591385fb697ff0799877e3bfab75d45a1900ca5769f8fc65dd890634f1a57b69e876da687b341180b7a38f62edd3cfb6ae361292ef2" },
                { "en-GB", "ef8a882bde3a794a21abd849aca48f7f1a6686ae0340e7e053002a0fa73873dba9e8e90d778590c216e7ad38accc34d376d0b0042908ab3c524fe1f3f1637d5a" },
                { "en-US", "9bdc8bd01a2fd679c0d34f07b6c9a432bc1d149a2e446c642e98b6fc017acc8e1d53c763a954f897cc1fb251355fb02a51229553432a040c3107a8864a3c1aa6" },
                { "eo", "d9ed0c6cbd0f21c3c711627d98dd09a10b70fe39423e52c756ae860eb6c5ffb1324b20244bbd20dd565b6468140427b91272f78ba14b1acd0e666ad11da79390" },
                { "es-AR", "76a68c812dd877fc11f6f86c13ef1b83fbc89517c64e901d9e47ad58298441f508ba28a8551ff61108b1d200d09ccc0a365c14d6bc5c39d599dbbab19f0cc4b2" },
                { "es-CL", "2ef1a33b364a3f209dc854945f6f416245d11b1f7208b1a33f132dd9161df436b61bf7a94eff04c56bccc952f922924402551011ac91e35c7078caf420d9b303" },
                { "es-ES", "7c954ee9fccb54cd777b30aed5ee4ff729edd19649ff9b060b5085210ce6feaa58318553bb910fdc2e8dcf3844ef4f6ff472be16fc5f29da262fda823b4e2c74" },
                { "es-MX", "454377b26a0f4f3ac9bb4ca568a0e31011297349b0a92b0c83897cb8ab28696fc8c1b36830efcb84b3220bc91d5136e013e18d7f56605f5d785e86e4ac08a1f5" },
                { "et", "4c02350da9830db8e2bcda6a4a3736936f46740c7627c1dcf22bfb1e114c5c1a3a4af395eb7283f2be04e766e43fcb7caf679523394e8b758f4acf2956eb6e07" },
                { "eu", "101fc39a03c7dc45a01fab36fb66d80056ca52dded304068d693c9b67f72171f87f48725fbce4ffea9fc4ced85b85b5f536735482ec9c9bd065cb38c269029f3" },
                { "fa", "c0c9ef748f5dfb1be6389503799e0a872c4734321e64eab2f5bdacc4bf5a2732e24e597772651b6ac10c1428c6ba00e7ef16054381c14143862334a9b0ac8f27" },
                { "ff", "79d7da9e73045f71d20e54807a07e4a61e18f35c2e5425a76b4bfe21f3273449fbb607f4b3776e7ee0e90784e7938e6b790a650a1581dad9723e17861880cc83" },
                { "fi", "8861cc0ef3258c497cf833f05767e6aa654770c13494ac276c10b8bb7a2b4a3c448efe035dfca7eaa38fbf6a38b1e2f9e13dc2f96b9fe71b1658deaeb874f961" },
                { "fr", "693e6ba36c775cfdc09441fabeeb85700c95dacfc4c1139b7cabe948a1c0779a6d007b88fc9d1e366b838da2c95e8d7292cf6262b601ce2e7a9481e6ad53a4e3" },
                { "fur", "07012dd5e8d686593c8c7359da0ccba9960184f20bf22cd4741f80c4a9c2157ab6b2e2a122d7b3e04e5eb644854099e7691f2da01e467c27ff5807177f067ae3" },
                { "fy-NL", "bbf1810484f71a7adccfdf98e536587e386d1faa0388ea1f8ac09100433c3f2e1a48a3e53b92bf46574bcacd727d4c30ccf7be16a08f9e259c05c1903db6d188" },
                { "ga-IE", "6c530d594415ec8e9b5fd4349e1d5d17f9a4552952fc9e302db05e772a61d0b223ed6198257ab518c8efdc05237e028cd0e95da589b15a93ee68f4112d367bf6" },
                { "gd", "e231699a7bbbd769818b09b84954bcd268431e5acb5220496cbff82350db40bf72e72dc80ae13530b9d474146986ae7823ff5a96bf803f3c82191f480a3154db" },
                { "gl", "7a2a3d6e0b7cf1b3db2465e62121da38b65ae5ef95bcd718e0ffa376f2317555d9bff5e6de7bdeb3ffe1be349ef41b527c33bcd84f56afe2d1bbb2120b1aaf5f" },
                { "gn", "9e51c570bc88562cb2b5d79be880ae6259e1a15d28d12349ff6fde7a6b38aac7eca329afc2081f2c7c2de51147a487d9bdcfe317af7bbf79081c1c3b00e913df" },
                { "gu-IN", "ad27f36361dedfce84ab05c102d4e96761c4f4ce73ecc97fe7c2b3719b195f0ce012eb263c965ab6be51907982eda931e68d11ce3407a5ef1e9d00e4f3d4568d" },
                { "he", "04b8fe2a72083ab239864846359c7a3a76914c5006dfce8f3dcb79d7844fd15d84e7cf6b46c4d9f70aa6e2b825a95d7bdc9cbffb72bd3c4c9ec4ea102664256f" },
                { "hi-IN", "a3045329efa0c02b20b06ebe705de03cbb2c38ed1fcd871d31fd1b7ac344ac35e770686fdbf724482313e3c5314d057df988b19222a5e1da19ad648c2373f45e" },
                { "hr", "a7c6015cb5c861388b4328750712f4c81b8de4b0a0cb03e9c9406aa7cf4c96fac579504cac78b159e884cfdbe46f5e1eeaa149435efa7d71d58f3929045ccbfe" },
                { "hsb", "33a1715906499c74c8b72a515c0b736ba10a93b1c6503f4940dc3a287cb8f948278bcc0a89b9e9e8529cf77b28f6e6d5308d15cae63f79f372534d23f68d5086" },
                { "hu", "cba7339a9878ce98358ff4012dc9b06e11d426b2b852024e30ddde1dd0847ff1ec1c023fb58f05b38d485417adfe9c0895e32d74bcbe14c9033501d497ff7ddb" },
                { "hy-AM", "7c77adeb164f54dc15ffe4add224d37c810f8516e75c1894e94192afd8f8dc9eeb5a3e88e42dd4198bd4993a66c0a5a61d9b04ef0643b55370a33d93e0a62f84" },
                { "ia", "42525333ed3009009e5430a6308c441624d61f199c4a37358047b60223ff132d292af809e7421084205838fc751e27e07718a37da16ad89b6b56a7093b208eee" },
                { "id", "719eefd971ab73e6b9180715315b2481f7c3a310fb5c1fc3962f124bf85017e1c38ae8236cd20017064004227a1b26b040bf82532e63fed003115abac1b5821e" },
                { "is", "e07a3637936d6594d6ac1cf103dafb94b45f0dd19f58ff4b59a21e0674afa2c0698529ddfdda88ec285a581b5ce29163340a7f7c8b76fb1d57516c6c84e50ad4" },
                { "it", "86731c9a24a6a4926026bde0c6e9769bc54dcd6d94cbf5ba982538bdcdecbf7d5bb6a050fc5d987e89002e184db1d0d98a36b74ea5b78e42feee3634e6ba7215" },
                { "ja", "231a548de68b9b796ad138c409e1592d9e3012fbda66256eb05d7f9c7e48c06977f899bb869b077cf1320a555e1866b01ddd6dc87815538219368bd0125294a0" },
                { "ka", "a3250b6d0ea149d23bf4d9005fd547815f6125add2c8bf8a338f4f7796ad70f808fe9913c31c965874a26f5f38330c90d0a5c9349b13b03452cf643723122755" },
                { "kab", "7d4d6371d9f39d5b47e30bf051ee03622099c782688fe64cdecd7a8582dcce87b2439ba940d5a7cc493e1a8cb6fdca18de494a5888618a00863fb0b200a57c28" },
                { "kk", "2a49bc4703536a5d61b83a39be138e338e006eed4ab5ee56ed6300cb87865602e4d4b0fac5a0fa267e2866b979320d8d0543bb1ec7e6adae1fb4cafb55e5406f" },
                { "km", "a6d324e816dbcd3cdc96ea9de4c5d32aca2f2066b5ec470052e3587254ae99bc4949976064202401a80b41a4a7a07d8400d85c5e5c644f5ca9e3819c7f334202" },
                { "kn", "758dc61e2eda9d2866be43844df7c89e1516874b4838803cf4d73b0a7ec110c667bf909c05e4d835bc89c5584886b03de7e9784dfe0814eff4c30785785e49d8" },
                { "ko", "cdbe8f0fab8ce1be9111296768bae4fda70e77650445421e92daebadd1c66e4a29fba5eb0917338f8c71c65250511a573c4bb2d877170e09de70d62eda741c32" },
                { "lij", "cdc1eee24c098ce0e62c519312e2139c454c50ce3721ab86a9000f07a8bc24bcd19a697f6426fc3647107c244f3f0b117ae67f398854b24b338fed080990433e" },
                { "lt", "fb881ed5061b8f646acebe5190f1b6ae38a1b986945258e8a7adafeedac8941ecf4ed4aaa5fe394e3809deb195a8f0f78ee51dbf30ba38eb5fc50b4cfe6480b6" },
                { "lv", "52bce4b0f4ab66d27d6ef9742910e20cb271ad476e8716857c72791df90ecc2a35e71b752b7bfb79bd7d1daedc5b18c7ed7b245ba448e81b8fbd98c385d173d6" },
                { "mk", "7e519f30743d767e0da9949e27eb4808f9ed65391146035601010d222c077ad805ef96d3f09374b4cd7e3c5f9c4edcba02c0944645241fdc88a02abff712485d" },
                { "mr", "a1e4c007f26f8f58a8cbcc2d6b1fba98b1af418ec8bcbc2d352a5b5d87630c038f4a0e82dff346037de2c332cd4563c11bcbf466d707cdbc221d3ad12c445fee" },
                { "ms", "4c1bfd6f2b49e1772233be94492ed0dd49d4df1305e9b6a3e8cdc3e9d4f427a262fd992be6a246934c4141524e3dec872844146392cfff22e3d80d0c166fc3a6" },
                { "my", "706f676b0c2db348dc4c8029750f4f270191be239a1ac883ad0bb006c3fc30be191851e9e5a9873152be199e3bbc51cf09c940f49a21b224f95afa9008194e1c" },
                { "nb-NO", "0bae6e6ba5a205636c3af54ff8fa870d13d586840d2788cc04020402e3cbe1695ce08c58a1b298a37052d158236ecc5933e87d5f4dd9ae17ac39ccad6207d9d5" },
                { "ne-NP", "7ccffb272f938e1d5533716a021affd94e38ea4c6e45c32056c3893ab5f83c0350fc10034ea3fd9199fd9df54928d9f81f6be20c2418ae20c9f7e6a1c5f0e000" },
                { "nl", "b0869aca67ab8b2acf6977d17fab6f2c96ee53588778ca475d02a2d09f2649af633a699cb999ffb0c401cb3f5ca72e56151f2dd52cba7ed1554a783f3b658097" },
                { "nn-NO", "b2c46055960cef92ad6d597f1132fb4e6186c509576a766ddf3dbceefd15c0f27f1585a644f652f949709e9344652b64ee31b0db2d0cd56bb2baf9a33e98665c" },
                { "oc", "af115a59909d41f25aa173931609ea75028c95bbd8bb028b1f956c5118fdcb4afb03b8a85f6d277f2c2ed563ae30f92d66b57c9820ad01bf97e3f71c17403391" },
                { "pa-IN", "ea23c5ab220d58cc328ac6499f1f1603492f83e41fcd00d8937cd2d98e4d2d9ba5c29954702591d8fde378ec069cac714094a020dbfa652b01d0cf6aa1ae63e3" },
                { "pl", "7699ff67e3e304fd2d5d8001aed80b645028548e8c44c0e8b88d7cbc3687a2c1fe1b169b3a723aca08581e2f8e4e7b063072a81c40c6a3bba1b27ab156b22904" },
                { "pt-BR", "0f91192624cded364b5bffaba07e292ff2f717fa5853185ebdeb2cdd3816879aba876d54a4e4e29ddb35dd07b9edb16ce5655ff65aab69a49484a33db08bca98" },
                { "pt-PT", "2b2b860f832093a907305985663617d6645ef4e30629dc7c87158d38b37707e98dd98665f656ae304a29e89ef9ff2733ff36693254d4c7e07ee97a0ea3d08ff0" },
                { "rm", "3c0cd62a4a8c5d688601f568d80fbbd0a33ff250386b01b4cc80aaeae047fde0a66f3a9e7fd73a2df90ae0e58b857b235e00ee6c09f043fbdff945c7a4faaa07" },
                { "ro", "95653539b4028b4eaf03fc10536cf0392e77674833e57ccd024a02b8097cc4fabffb4817accbbc0bfe9847478707a6dbf11f9ca490b9a6a53e009a8278281612" },
                { "ru", "c42d08dd82b5738e8f1ccd8c852adc944e0eb92a13d178c838359dde29badccb3207335573985b19af4798259aba43a8c8efbb9bff2883383c6c7548abe9d915" },
                { "sat", "4119b24bf2e4de31804b630d215c151045891cfbb856231c3b841079865cb36cebc89d0d4e829fc3c36504ca803d7c2b00ba05a15b397a191c7dd0ad8d320648" },
                { "sc", "997e71fad8b6ecf06850c9823dac4ed038ebd851643204492b7f5b221faa1fba1d6fab3470a00ef6e5301d1c2845ab4735d0a15d9b9c19b40181dd863dabe177" },
                { "sco", "dd6c4876b4681db0b247923ff61ba0b100a640a84bf0eb37852f0fcb794d111b934d21e48dd1093fc828c57d5793e438c330d0823de383645694c61004723458" },
                { "si", "9e41ae659966321f9d62ba30c90d59bc0e43a04e8a20abcad8afaea2ad6c4c56c47fe80745108865add129a9ef250f8562f9a1858910ffce3134afdad4462c46" },
                { "sk", "a26f4df47a2e79e74d09ed2e2e597b0b2d8ac350c19f0c83d2271445f78cf4b13e8be1bc2db921ccca2f056bfa9dc5abbc525d10148086d6db07ca6cee23336d" },
                { "sl", "4c1883a37496d723e9070e88104564f291684b5bd1d3dc0f97a7f9017291b330e2586cf8819fedc131e90e32b13f05dacb14e6ba0f454ec02ddef8d8cb4c751a" },
                { "son", "748b5096e10d224dec5096c2882099169106f6910dccf3a8c9ec97c7dd0148f1d00902474f9c4cb455e2a9cc439db586e6a7a377943fa893da4ab5aadb3de004" },
                { "sq", "2101a864d5e400bd04481c9d2189631e107b342894f229733960c6401f5e459992d8f2e3c4d4051806d181f63754d33ced3f3d9b62160bb54745a8b1364e7e38" },
                { "sr", "318860180a43431894e151431b603ec584e017c6fc5a406fa30cf1f7bcb0fe644f1216d1c24c23bfeafc166cd2088c95b77a500334a40670e73a2461d995f669" },
                { "sv-SE", "1d7e24bdae6f6e21f4a26646474772a286916bdd3b739530e418d96ad1f13ccbbd7a6f9117ef8926c511e05d7cbaa0891c94ff6ba55222e3cc21388c2152cce0" },
                { "szl", "a241f32714431669a5b93d7b46117f3993484437d6ec9599416b65271a0b0b6ee90ff84ffc5e5fd24ce8e240a1f819345d39bef10e03bfdc3af7851b9d3e52bc" },
                { "ta", "aa0e60d2f498cce0de9b2052a3edcef06d14fd501c62ad43e447fb99062a0203a57493d15ea583bdd20bcc8c70f0a769ffe3dd9c3518c3a189ce926e2078752c" },
                { "te", "1eab99067a89159575023ce183f7cb2a0840d8142141eaa46ed96ae65b0fda3ee6bf642a93a4c73ae9b64f0a4bf7556d72e8e41faa561bb144671f0385732153" },
                { "tg", "bae2c6fbde64268e6c81372bec6803d4499ed1447976934be303c00d4be387cc242576d01e6e70ffd79dd75c41aeffc811c9864e7ca5a14ea5937a6f779aa2cb" },
                { "th", "cb3253c5234e3ae59d1cc2af0575183cc703217d022f8bbd48b4da1f915575f044d20c92bb1db4a6fd56afc96634732de4412a8b339a891b7f96052752962308" },
                { "tl", "7fcf7d5ffcbbe943ab77ac3c65ef6eefecba243e59c0a119620f462bf585deb6cdc8c41661db5b38b20478641b01d20faaaa37fe8c43fba98b0a7fc470ce9ab2" },
                { "tr", "0c47389d5eb02c1f8e258cc85944071801c59bfee068076af4ea8d736319530d21c87898907fe8220b64e11aa111a875dea543b2b9a5da83a40c8b344b81d152" },
                { "trs", "ad8566a278d8e13251aa7b00e22bdeefbf9173fc4e8941974d7da72e253cd9909980ebaff133641e5beab6e1c0f7b3f462f564bff105d7daebfe044d3844dc9b" },
                { "uk", "46b956b100b0efa23bbf6620eb391f59da3466f40364f9a16b426261a00068db3f35ee2101c26d24cb930f174905c940afd48f0e4d15dbfaeb4ad6fc3479e555" },
                { "ur", "e4fa089d96d04ce6b97cc9a24978d11dd4f3eaa08f3d6a7ace95d387d0a791ba194a83f21ad0bc78ac573d625ca747d108bb3735d6525538f5398f18b8538edd" },
                { "uz", "6a1272649e2cb26d63c8d787e8ae3686218dd84954ec963f860316c2bf9d6544bc9ecc945b1cae3ee00b528181a5dfdcfbc735b8046ca60dceb5ece55f918d6f" },
                { "vi", "ebff8cb7d2c39b580708d42be69248528026c1580b54fe4ed72245d01c44a8d6e72bd7634299033932391f512d1acb03fc2690dc1f83ed2733f3da81b60ee4e8" },
                { "xh", "7d38af4e4db37da551af2fa679163b96154ae4fa3a237115b216f5f5c8e629655b58c905a2f47ae0b0162ca297e1b10f00f492aabb9acff172a1bf2ee0ae32dd" },
                { "zh-CN", "b9ed59c055dc2b4dbb649d9115437b49e84a3f03f7bdf6baa2135008b4b29360b639abc0e7157d5cc3b0e949abb0d2d38138713764cd9086d0b7eb1a8b06b329" },
                { "zh-TW", "16a5fd9afb94e60826d6f5be82e77127ac665b843ff6468ede1e2d19b6a9456435ff9897340a88129e4a57a33cbc7b190f7ac1d0d5e7dce022a019d475a66dda" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/119.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "9c553fc7262339847b333167fa63c79b65d67854e69dd8f5171fc965af7ade4119afb56775f96b7d896c1edfa54143786da92552b003014340a76a9c8bafa8b4" },
                { "af", "7a9988b2a0ce6b852aa4e7bb95714fb669737d5b0bc904089d60eaf4e97846b70023eb3d75da4bb5582d0556545f4b620771106b6b146ea1854311873a28e341" },
                { "an", "078763df3ee7df25e0c021476642e26abc338af66b843bc45574a18826813b4148c38739f445ba70dd0080431a29afbe6292bd46872df9b12e9562483b08baef" },
                { "ar", "12b26a7f84534dc1f56cfac913e3ca15f461fbb9b985f80285b90249355540f060269efea90d52f738d95df6e30cb4ee62da2911b0b175c0b2b10467b1afd9e7" },
                { "ast", "1108c993e80eaa9f6aab065cfde8bbab0e7594253d42e28ec4ca5aa247b61d46f8d2658607f687f2fd277546e1263d8b745d049e171e341a5287cc8412f18ae0" },
                { "az", "be6a7a6bdd99e5951b135c2879eb0335f177c42651a3e22033c1c311302ecc6bb23857bd3c87bafbf46b5522ae7e458723113f03c7b39875553aff7a040527fe" },
                { "be", "fe3f345f5122d1b9bcb0a4c0841d710b799503d1555a81edcfc287dcc5861f45cc103e1692b8463a65ee6f7834020f54b6445870117579f1e7878723b366b767" },
                { "bg", "4b586099afaa9065e721f8702b00a713b297e257f3a71861ccc3d2db9b8230d24740b4b09a58d26a18f5a8dc8291d29140755195f049c60c271dc7d57d29827c" },
                { "bn", "f0406fb77a9cf65923af18541e735747ac24a2517579c58a7ce3aa0ffc952d4ae53587e63b17b59f1ffd453d473c27756ec43acb858db4d3a8215ce6883262f0" },
                { "br", "68aa23c67feee46ed9cfe158d60f161591a824799aae347d125682d89d1e057d201ca005d97cabf7707536f38e41d686391d162c1f9a11a8c30b87548cb4cd99" },
                { "bs", "b88759293aefcca97d882c1d5096cab681b98ab76a65fd7247f7f649bdafb8943507770f31b005e3ab5612d676590617a8abdfec592033abf4b6e8e876477809" },
                { "ca", "154bbd6be171be19fbc1d954426f15d0c0479d0c8b9e3c4c8347a3bad1ab6dbe60f7ed11615d88c0f395c1a7180b561d661c8d460581304e46753357dce86d35" },
                { "cak", "18b017c4098baf50aec797b44462edaf4dea5135e38635751d7e4e23c5a0c344fb911b213f88d70aa78dee69a3f1475163a391af2ac167a131d6a836204c3ac2" },
                { "cs", "bb96c7d1f6dbcdb2c378d54fa32d0a832814c86da91afe9e1ceb61f16228a5e39af3b3ff6e125984095a667923bd05bfaf2f84072d112e0171728806597cc0c2" },
                { "cy", "786fb919d99568a85601558d10bbf8e680c432c1b9a748e346df1dc8cbb9e764d254d4d99478f6d8605a27d11b32f11c098694b45bf25644535497b9f6ba69ad" },
                { "da", "7c1e986c054dd0f09a1cb706c5dbac72f5d94b35cd787cac630d5d261e78746b04dd34931b540a3bbc6291ba7d274f5b8f16dda219aa40e58c720d6a16695cdb" },
                { "de", "941a33110078e8bcfb75b4dd527ff11536e6c3dfa5215aefaf18ccc7b7a889f5d5140d5e47c311c28553befaa7d7ddfd28af85dfd646d3b797518e6c8b6707c5" },
                { "dsb", "3abb311619cdc89811e7a6c77eca7861377c147c0b3001bbcf4da9bc61268ef417d02488f33ec1cdcb055e7c1d74fd139287102a21f84505880be4ac558ef096" },
                { "el", "8f234baf033f0be8cfc22d72361ee8089f2f137416fdbfcfc435268f5b04ddda5dd91d5058f0f004f39e81170726515db14aad07b0b85b01637a866674b9f49a" },
                { "en-CA", "4f6f2a9fc201962fcc9def723676ab2dcefa40b519534d4e7c70bc8daac8876a5abc756c9670ef9ee7f4b2345b43f93fd5d593f1a02265e6ad5db64407299150" },
                { "en-GB", "62f26bde7eaebbedbd1477fb5635bcada51df85ff2b427226d10c96ba90eabb47a3d6eddeb9f2d2abbd5a401a70bfcb7bdb54bf58e1f02df6bb89a38631b9fdc" },
                { "en-US", "99c94c5c02e481e51e58226120da91afe50909068981264287d9df50308232ac29ea91b26beb72aada9e53c186e1b084597c114205fad0d52568b011830bb1a6" },
                { "eo", "d44f85793461e59126e44aa545de27f19d3075d366cab8142a2ac4d8cf3f874912ea090a6ffe4b96bb9d5682dc467e31c110f93a27c48e5cfaa66fac478b97dd" },
                { "es-AR", "8eb39c857f50b356fae954593232de8211379c51a67341ba00922ce19d3a51e02d9a751ff8f7613da40432e0123cdd772a8229f223170118ca1fbbcb27be8633" },
                { "es-CL", "f966b98421c07858ac761fcbf63710411289982062b08ab6e1a11ca9c08c7e8cc0cc8a9c7707b41ea964973289081c57df0270221fdf081ea82bc72ea0057f12" },
                { "es-ES", "2f04e50a109ee379b995fed4a4a3c74b898b482c51f76f13cb1ff4ce2314fe28339fd9461224aa2e3bc0ff8328e83145c42b1f689f710ae1cc58e9327fac1c34" },
                { "es-MX", "f2e530b82207ed0899b56fa1c8e0ea1b056b70c803fc0d0353de143fc9ede6fbc5328a88206891a26b28123a831fcc7b1d436dfd5b503b51241fd87fe89ae3e3" },
                { "et", "07f4d9384ecc257d956d49f8de8f6ba2f8b9a9ff4764d94739c33d1911ff8ccd2cb3e196d7516f4a856daa95388ab09c5c415974f3dcf35fe1490227565c06a9" },
                { "eu", "f01cee2ed07a3962f8f22adb923772e4d27481f8313dd8939cbbe29ba8bb37f06d2205e16706c028b72b4cae4ee75f8349a4bbfb84af4986f96638f7824b9af6" },
                { "fa", "53ea2efb78779101f96f79afaca85bb31e24bc3a611a6d1597925665c60cea14d8683722b753a15b6a4bcc463da74c0fe60d6296fa3c08adcd63dc12561ba3ff" },
                { "ff", "6716f3decf75ce2b337b05e1e659d6e1559521af6f4e2e24efb4aeb96f7fe0124f9994d5cb93eb42a4b1252548699ea278a146f71a645395abefcd7905fbc3dd" },
                { "fi", "728d3a05f6c149d17f02e4233b8d9fe1a449f047df228ae71d1a3a41ceb3a8add7503f6ebe733fb16c7fad3fddc57646dad9a3dcae035da7df17d378b556bef3" },
                { "fr", "9e18d494eac467ac70f7ca08d4cf91552d7d8a1fcfec1e699341f4802c51b4dd6d58f6904f07bb9f610938f6573116232be2b427b804fafcbaaeafe6ba6d3384" },
                { "fur", "5fd735072bbe9e7e53168118b3290bec4dc8e29556047ba736c0e410e165756aa2b96964a29b1e0fdd7ec41a512a6fcfc4404515ea2f0de10dc4be0b471712ba" },
                { "fy-NL", "6360c454f9ec5b25a5fd9376fd4c0e679c4c3d4c0f3bed7ded233c2ca9bcb83b8569576eeb3b09a1b75b6795be258d1faaf1392d7db04ecd03a25da28d1d4802" },
                { "ga-IE", "587040d3bd3167ab80b5cb875ed728d107ba85cb172f654af66e409bb282a849285fc2f8842cf1350ca8d33d8d6db805bfccf4f96a77c80f6accd31856e3886c" },
                { "gd", "b0432df91eec0a5aebba7cc6fdd9552a83b0cd4329c41cbfbc3aeec00729433a89dc60d292559bfa6bfa9f957debb1f30c5cd8aba40c36d4d6c732db1d28cf37" },
                { "gl", "81be8b2ff165d6d79f9bc8c413a82534f9af4abcb3c3a9a3a1be7eeb04376ddf2f27377d4f45548c8c3ae8e03e26a74aa6a8548dc29ae96a3e9f422b300060f9" },
                { "gn", "6fccb85256443a57c6431ddb5346a0978315f515303b49ff7bc621c1fe05079134f3e0e9434860e815d867e9c174e8a957eedc74a9d8fa334930681c3d0bd21a" },
                { "gu-IN", "e3e8ba22c1c53aebdae8df46e19e9316b1d9d833a90b6849cbb2d8631f9be358f5f6d886add6fb8628428171356f66bbd0dd7927a7eb620024799fd48c4b4b0f" },
                { "he", "04903ddaf7f9234ff68845020445f57c640fa8957d07e0e8662ef4f3a6b384be667f51fbbe2b1490b0e4197bffa89ea73e8556daccdd11514f827984f2320cf7" },
                { "hi-IN", "9dc39f5707d4fe83020fad8d20e6b3a6d373aaf3e04d839711928d54d38af4808661fecd45255fcc40bd27d6ffa9cdeab0e6d88139022e18362509a7ee61770b" },
                { "hr", "f43ee9400bda948e999b9ab66971965b445a58dae5e20818da3c68d12ef1cc004f3d46000d8c011a294f8db5126b05cbb52330f5a4c877b293e249ba165b2d5d" },
                { "hsb", "ddf832252912663da43e85baf9ff75c859f0d334c3bb73c3f37b49f55d79a262cf0d96acbdfc03d80e85d862caa3dd39d2d33435e82c4465152148481f57460d" },
                { "hu", "9e20b1cfe8666c1b5a4f207bc12f492921c51ef10eec59e4ba58e25c49447266480cc27f2ec33d256fbdf334e79455d44ed676f9180a10e3baa07b567ad00d67" },
                { "hy-AM", "282fce8412a17acbb945af30dde6316d9394b0e4faf6eae3b585a0d2c73f4c3d125b079bd5a04065d9d0311fa8d7efa447525a9610b33dc4b0a3c9609e0531b8" },
                { "ia", "6f875f2500a5af5f85393bb6d3b5e240aca8a2c0bec8464a6296cbcc5c31948d27c366e495141838e0c7f0bf2eea7bcea6c649f6364a291a141eeee23ff5e4f1" },
                { "id", "b9175e49bef5312de2db69f087d06c923892e6a6a96f0b254ed2debae1d8f60f5a8edf7b67cd0b272ed5f018673a2cef39e0060ff4ad6d8890f2347885a7f207" },
                { "is", "ad3c0669fffd7969ed93f71d9c71b579e4715d2092c3ac7c63522bdeffb2c806aa5bf2bdd8c72ca09b59c11ac282e1f273cb09bdbcebb60a1b39e51ec9933ae1" },
                { "it", "ee62d3b7c5e332d271823f617a5c64d345ce534f1f69135e0994fdd05f8c916e1e04d517faecd087f3ef9ffb26f0f2f1cce3ca88bb3a57eb4e85181db43eb21e" },
                { "ja", "c7982a08b59e3b225b009e63ea81e8f427d6382d21f151db8a75b1f57152777b56f7810df58f1338022b081db1dcc6d0a1bdb7b5925cebfda18651139c7e24a8" },
                { "ka", "7c3b4bc26b8d9db81694fa0fb29fe49990ce227f8fe5068500d7d256b5b2d9df2dfd74b01c5bfe9626f007ff40e11bc636708118c6164f52cddf746d9517b9d4" },
                { "kab", "de6c850b09319fa48f59fa3901621c25fd17fb8587f142d08b6f8607833021b4041171f925216b70e90aa6b71ad91e1dac769d344c20dcbfd9a9e4bb93023bc8" },
                { "kk", "1f26220624d9950c1ee05d8c5202826c12d5662d931d86feeff81176b1c9423b451f14726ccc39458c83d6ae289915761d405a9ecb9ed9ef9718deb5c2b91a06" },
                { "km", "f1739daab8ef5148c5e1dbf37f72982e54f15e95f64b8d3fd409425548a510a1b605188f12d95e35f2802c279add96bfebd04e77aabff71afef91b59ffb92c02" },
                { "kn", "a805393a0601aca7eb30dcf6c822a2c7f88182dff7b2f7f046a07ca022b6b08596905e05dd3cdca770e87f899cea6548c728cd5cc29e53c2cbafc14b0e199fa6" },
                { "ko", "72266a63f81cd2a329ecd1faf425b915275c1a2097039585cd8d938bc71fc1d1478bce0f11db8b0c89eff2eb20c59b8468d33d8cc4cc559618007eda0a8065b0" },
                { "lij", "7cade19baae5cde997697eabc360da8c3c0ce3785a9e89a54eac83df7493bd9d39ca4110acef6e1df906f6090a672eacf4027d2e1cb21694b0f4e6d3a10d1632" },
                { "lt", "6f04a1e0aea24df57ccfe0b08f453f027d70369944404c606fa032888e7cd845bbf385462288b212caaf6907ea81d38260de4028bfb0c5ab076579e63e7c1b94" },
                { "lv", "6b2e1a44c23ce0a619b850f22a35bfe0582aa73a9703355f919901085cfdf345532a964d2f9421632716d76ae9585e8e35f5dc96b8e9d1aca4ebadd96ec8beb8" },
                { "mk", "80b1c9f6463ac4e5aa06ec3aaf5f7ad6ff3ce9449f3e59e2b0d0c82ca1673c58eb6861fcf04403eecca31250bede19497028717ed4f44840bd590a53e5160734" },
                { "mr", "58b52b7d0d262768a8337f72669b85eebdc1c7ff1b680ba7ab6e4bc0b052b46165ee8a0bb85b7bd7b983e92b0e2d7de936fe1e0c4edc25ff79750bfba8303f61" },
                { "ms", "34e87928482b6f4d8a143506319d733e217d3312d25bf39f293392cc8cf8d9dab58bb0b85eb8bdecd48239dc5f08d26a023f44e5f9c9d58e4832e95086f0d555" },
                { "my", "c949fee0554bd98f972ca83bc9eb692b983e3284aff088b23012ff900afa66f1b13b00064508623e6e66ab2f00b9b0d5c3fc3c1d599cc4fe5ad53e776fbf6eeb" },
                { "nb-NO", "bfd36f322a67930acdd3d25df54f40f52fd0480cd361d68567e20fd4c2df4b8e507bffc680b88f25519908df0918f8cb381a16974288aae98fe3b36c6d94d458" },
                { "ne-NP", "7c976a63b1d497b1039b6c98e9c61d3b05a971ecf15fb31ed4df69e63f5ff77350d231acec9e421d3f6e0768337f1cc91bfc32e7b47c2d9de8593c018ca8672c" },
                { "nl", "7b048b032a4320dec2e1f85b55d40ea81088f001fff6161858c76c50b022d359c89b81c45b536608cdda8d50a0c0e7319eaeeb2acee913bcbf64a0ed492c628d" },
                { "nn-NO", "1068385beb223f5036c34db4ace416adaa92e48bb42d861e206bd4a7da041f6164c078b254292c2d8c95a56b80c9f0306d19b5d6b532abc264ae271040d1e5e2" },
                { "oc", "2ee2e99ef604575f40a24d2fea8a6bb8fa26c7ff025a3014586048a80e5a07a02ce6526d0f70fe812eea1723b018f39bbcd53b67d908be52ca9d9a55ff9e6097" },
                { "pa-IN", "bdf3847af1e5224759f92b9b815af3281478b7a7fa203621ea47b56b21b0d8e382664e927e0823809d4d36c930f517acddf2ee084b8229c034f875246437d691" },
                { "pl", "70d969ac987dfe40269df994d00d7c193fa8e3944cbd389f2f40dfc2869206ce901300dd1dad072452872d541f19874df8bc22e1b4d49aff882f525e1ce94f53" },
                { "pt-BR", "55eb0a5bc308ccc0d350260f537f06c809ebaa3ec0aac0612bae3937fd8d67174365f5edfd07804efe11048954f64a3cf91056950397f005a7c611b8500d9534" },
                { "pt-PT", "72fb46f1ee161fca371712fa72b6a7d33a71efc5d256c0bf1f5c398224c70f094e28498f9e17b7cb6ec5d4f90d383d84c4deefe9d70663da7f5bcb93d0244b53" },
                { "rm", "03237e936a4a06ac39a8b31ad20cb8dd55c3f738e9a00a6db3c4e323d1da2b7369363c0bc82875eaad584852b3ae9705c8babd1c4efb527ff7902180ea4c52f4" },
                { "ro", "1fa845863b8a2876a37830bc499cda5b3bd1b37696c6fc626abb2843a15ee1b8dba51194d130450430d6bbeae5aad1592934f54ae34f68647cdb3bb52f1bc731" },
                { "ru", "fde110b8126cc75931673c67674a3e8c0d6c272c6f6d9458ab2fd169ddbb164ebabd48f6dedcb744a1e1b3790ef392d0d818d0284b3207ecc0f43986a0a4fe87" },
                { "sat", "01f26ffb6caf0da939baec6b3b0a27dfed3c763ee4e545f94a88871c01e08b76946d6f7ae3ff34ca941daa249de10b9c6384042e7879b1c57a2e4c2aad98e124" },
                { "sc", "ebb23eb411a49d93405f167f08724e1c132acc9fda4abe84f9f34b50f8bc6804395512df6cb3e479dbecb206701afc01e3ec88b409ec2a2aee07514a974f0eab" },
                { "sco", "36a11212c3f75dc7988fe83531e1fe2369b6991368166ea10cbd34e231875fa5f857e071c6408ef72f1f13e287a9d847debd5462bb6bf0cdbfb9e3882782eed8" },
                { "si", "940af32788402e530198777b993381088c7665be58a3e5532a3e5d3d13cf6014c390ad201e3163b7ca1e8e203b4c69ddd02092f01f4d0f5f0a4ea6d0764d4e9e" },
                { "sk", "3ef1b5a8e2206d0d74f36e1cf0ec4d6985aa853d516561beeda1940b0fc2fc65200bc8e94fc6374e807a5ddd7163461b378b525ebb16cf0e3deb250d6bb8600a" },
                { "sl", "a272af0bcc89da0babf8ee558b56a0132a142c0747465dd0dc20a58f1cee7b4aecbd3c0c2a285ee01f7b614aa63325eac42472fe9092f6cad45a93a9d8fdff71" },
                { "son", "5040a72a2819464d79a73c7e42bf2c7944acac760925cb32af2e321927a2bb350ac0d7f1c65ed6cf7287f6c84aaea04744ec9c9c06953f940334fa9a1619e398" },
                { "sq", "cfd8094ed7b591bfa6dd7540b9a85b6e3d1db7d638e9e1ad9a49d71d24783e906587ef1b5bcfa10285ab51169c1b754bb20173c83704d4bbb3d9da4fecb0f7b7" },
                { "sr", "04fbfb2baa55e1e6fd6f146377fb15151b097cd5be07ee5fc21617febeb3307126f9d008ecbc87dfa0d4ac92c036ac42822a79a49578da1ae8839acf840a4a11" },
                { "sv-SE", "bbfbaa7bbb7ce79f203e0fec76d180bd40ee84fa60b1cb30b06884dc8acd67eb80edc786c1f0d9e5724532c3f412b573204e21b4b6bf513ec2245f8797918624" },
                { "szl", "9b9e52d6dac3712ce2241af09d59559683b1bdc1bc3b1f96f8310849bf5bd208f5c3135d3792c83c15e5a3c5c0911c80810b0eab6c4e410f10e43f2c95794506" },
                { "ta", "03cebe2971eb620071c30ec9ac2f244689db39e51a0c4fa7af85373833e8ff7ceb8361c0a31f02b0c2596a9ff8c6e0690d1c2f9669491e0609e91e65133a15fb" },
                { "te", "163575a5794eb78d65cae095d78a7cd5426f6ccfe4aa49fe0b5e2168959afebe72637767a4549909071842dbf17b41af79160cf1784259e272a1515e6f189ad8" },
                { "tg", "390de1cf82eaacd63569d906f318cd5400cff73d5df956e6f5a6ce7ad3d359d7113a6b91798f9e2a7307130da21743a35040c00ca58b4c22c6de0b388dc43319" },
                { "th", "4376cb821461c188379c14b7d9f640749885057348a79280d155e0386daee01e336b58413de0b78fd691a92b2140e9acacf25a0a2ada22bfbb4ef75000a4ea55" },
                { "tl", "614249cfbcd1100b4bb18b1a24928d6f4b883698cd081ecff650e57423673a2f1ffb5720c48be287ea9f6f3966f4577657df111ddae6dbe7cfc788ad5ef2b57b" },
                { "tr", "3c96cdd2ac4f83efc08f3e1a3461f3f27bd0ae793b9689280b828b8736200b8fc676cb8a5255a88abfb2c4e013d26c655aff330b229b08add0171fdb6e39aa2e" },
                { "trs", "ed17550d1a6a68803452dd83ab7576653c28a5717650d2cb03d9c554dc6ce24e11e46033053b74fdc9933630d4b623b08090a3d3a72d19ad9ff171c1b4319915" },
                { "uk", "08bc9e2b151a6252047ba4609ca16f2b3de80b10e39a30eb5e469dbf5f3ee51ed180755e32635e29d0799057f026cdde43e7819433b6f3ad3343bcb0eb2b3370" },
                { "ur", "a5a972fe18428b3e3d9746cd92b8f253b97279e182eef35e5609b7b7d21baea779ac3f9425f25193b10925947a32394bb087ce88a0c72939919dc283ab1a96da" },
                { "uz", "34a9a892bd5bd2f0a5cd4f8cc5c606ba2e2f0a36d9ff858c72184e04599bae52f1caeeea0f332367fbd60c34475b2694a3112ebc20926cc8ca9dc4d35f09e5e7" },
                { "vi", "6452856a379f358cd7c51cbaf327d9d666cd1aed9fe6754049aaaddc10b7b22a84ebf1d5bc23c0df74d78ab3a5eb128690a09516d5bd1847e519100aebe2a71a" },
                { "xh", "e5e73359243e875f994150fa07d360829e0ffd44b8dda72f8577ffb06417e5b0d93c6309a46e3d6126e4eae82efb1ec515b79bf8c256f3c02ff2ce8f6328a70c" },
                { "zh-CN", "faafb02fd17be38dbeb4f5cea35fd950b1703cf3a50917f4c08b5bcda37bb8cbe1f2b68cec38e2db840355baf945475f1ac40ee4626e68fac792bce31826d2f4" },
                { "zh-TW", "9c77759ad020a59fd3863713a11a541370347a538944e8898ffbe78e1a5222dc1e32dc8b7b12fb6faa9e7ff3bb9ebdcf09df4fd86b411e9bdd40b2e0b43a2795" }
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
            const string knownVersion = "119.0";
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
                client = null;
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
