﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "135.0b4";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "c51c8edd0b2d28292044ef687ea7823b1c6acd7310a2572a1786d353509e83b4cd894fdb0b35a0c6584a1bdb774f4c520e429ffdb25095925e06f519c2593a78" },
                { "af", "baecdfa141190a9e1d5b884e8e99f018abcfccc0174accf191217f3abf147299739ea48b03a39ca4e1c09954ca4d0bf74fa073e7e883b436ff6e9b0b1cd35a0d" },
                { "an", "2f445ba2a62495d08cd0e876d38498b31fd18c8e47ab43a099162513dcaf5ed639e3ce07ed9af969adfcecb71cca951e6d9051e93757029b91437d9dd0d297a0" },
                { "ar", "fab047f87830ee5e79a768e25fe0e1f5d7778fd78a2588eaff7ecfec94d858d0591f783f7916e3bf101e518e7b3445fed72f74e3871790a33627e9098663a923" },
                { "ast", "00905736c1be11d8abd9374d50893bb182240fdf3078c47b571f2fb35c51ca725d3067e7810fb0589ff3b8c6a5dba03e8253b85b056842095c41c7dbf75a999c" },
                { "az", "3eacc39af2423f1921852f7f33c36e569e453a8efdf72294b59067b9b13efb693e0b2d6c930dbdc2491e6238a7a96901c7d2324e08c7ef347ded2e70bae65aa2" },
                { "be", "a9266dbfe436921656971be58fc2233c46f7d8b3738041d5cde52b4896ed29e6298b03424d749ea11873ece54adf002437b049c84ebc9aec764706b8f938b22e" },
                { "bg", "14b3cae6f43b2c19f0c7271417744832d18f4e58d69a429ba3f3a2ab8567ba110c2f2423ea38c5b234446ec98e6431764b54e7cd9fe6c2bd59c202351e290104" },
                { "bn", "7b25cb81c8f831b8381ef8c7a0384ffb5c67a0afcf505b8ce4d492ab3b30423ed51dfd91d507b2f3f746e8d5ec150e3fd23e89ddc27285f9e6a28b39ddd359a7" },
                { "br", "39b872e865bcaeca309a72b59af5267075aca7c29e87b18ac0fe89664245b36025b9e7f9e6bcd55e0e601a1438fa4d2b7b2a061c5f02a327f15598ecf45171fa" },
                { "bs", "e88799a49de74fa6e70eadc01ce73c994e6d10a12b5c996357e573f5fb350d3ed1c5b0c3bc56549c6e7579323cf8987ecbf4327cd601a1f9bcaeb03d3dda55c4" },
                { "ca", "9024100273698e33f03ff1761f5b1328373f15a7e500e193b9de370e9998968ba28721de875c7f6193b4cee9d344f83a148e1c27a04dd448f076def8f4ec71e1" },
                { "cak", "9bf6d8d8633b0808257aeb2f2add1175d51df885c32932a3495e24c855fad8b5facabde2d059b6fc830e2f421edd8adae58f13278c32b4044d99646478397474" },
                { "cs", "210307839b4df4b637dba8c7158cb194a7cf77970403f531e78c2d6b617d81b4f8b709bf0854644618987301a1491a764f8f16f176181e9b0a8742eb13fbbd51" },
                { "cy", "24c7825193f281c3395011ec0d7d4d011993a45c019629c6699096574ca0611ba756f0d2838de3a32172242a07b12838e88f304dbd06eb51fb657304366429fc" },
                { "da", "d5bdc0d871de5935cf241c75eb5d50e98f8394904411e1ee98b976de252917c7005970a3fc1f131e41eccbe29039d35594278e691d8ccba2a4913a0d5e24002b" },
                { "de", "e99140557b10a870e3e8416305ae88deab67a5255b8a21e5946b8cf379fc23a1be3dae7e0de31a99a9eb987ebd5b769e2f1d7d312eade62591cc5b67dc7ea19a" },
                { "dsb", "2d88d3b5aca015ec40c1813621dce9165880b63a8d76819d5bb52fcc03c400c9f74981a595b30853f33cf64bb9d3c0d8788ee0b1b25ce0ab05f80c75c3db608f" },
                { "el", "c4ede99fb3fe95a95d5fe34bf27f427aca1fd7c7a7e8e501610fe1254c398969b0b05182e06fe5b4d892e0ee5fd274b07fd1b18095ba73cf6adcd6d08a3be5d2" },
                { "en-CA", "eb0b788af2b5e41540dbfadac7f5fa71b4cbacbb23aeebc1bb21c891d7b7014c0d126927852098280dbc06617180c84df602e539ab06e9ea700e1f570584a0e7" },
                { "en-GB", "142afe4d7133d8493b7f189e6861221cd619d0dcc3d965969ce9d7092ae66b3825db631e4c749b14fd6fa28dbd042925f685c1be6f35a48de522d0ea07dc19d1" },
                { "en-US", "2f975503e7b559271635ee667fc683016c79ff59f26f5184ea3b23f48c81f9b50c1a1d434aa52e252b261737e869243b20fdf8441fa187e1f301e8d069ad7189" },
                { "eo", "20db8bef7d7cbf6f68b62429563c14c454dcd6a7bb4f6f3a8a34b0dbfd724a4fdc7c2a1a4ddbad50195bad0a8af517f0cfe2f0eab2d98e5d076619d96fa0a114" },
                { "es-AR", "0d2ab32830a06d2368811b7c23f71df4aec06dc29632a6c1d2b7ea1b47a13ec3d61f01f0c02ba0436651c80920b01615847926b56cfb4dae3bbd571189af04b1" },
                { "es-CL", "b8e9063fb89a7faa448337d3cd74616e1e86e8229d765a769bb20212f8a4040f22f314d1423c43180dcd913ede29c1b82173f41d6639a2342696e101002cef52" },
                { "es-ES", "53b9530adb952361fec816edfe58632d3aa199f9d640d08a40ee5c61961b02bcb09297d80c2d92c1a5a211c6eae421acae9a08188d1f6ebe900069874d42a0ba" },
                { "es-MX", "7da8944c51857d3a5f55999ebcc93b84e427cd82894e6fe24c5b7fbc2e0d524b46b261779df78364651a6de5951a98956705aa0644f9bbfa4b2f2c3105cb67cf" },
                { "et", "b6885e245ec2fa7d0e04c2b32e164051d657a59f1e15a98ccaa4aacc8417e8e6539757857005a338a39200c89c09130823e26f175dad3616345e9445b889564e" },
                { "eu", "6bccc57f15c316d0e23d812d623997b4f2c8aa2d9368a31abcfbdec9e069302c9e2de6c7035cabd8a0b537f4be87bf3fda282a00941535afdad3a3e0c62c88e6" },
                { "fa", "76cdc4c2881053f6de24f0147b7c7176f17e4690bf55e13453742b0901b17257c1e71a835e452ef1d6228c8ccf8b61e668820324d9f67ae9fdeebef2a21a4f83" },
                { "ff", "8554359695f110645f82aad6d56539365f0b4bccc4a1edd1f8ad161a8271ee3a9567a7349e452a195aa7357064b7bb8434032dab67df7266934a5bd3670c08d6" },
                { "fi", "8db33d8b4c603c6e9b49bf398398d287f73c16fdd041e5d8d69e36edc39a759a45617b196abe07250173063ba73f0ec7ac080bf9147857329b41f959355bfa62" },
                { "fr", "1cd9bd569d81ceda56fff4104e919f5eca655fed5f9feda1d9443fc37851332465f4cfd98c508b283c8a13c62cb55a4f4c0d9a8da003d45fecbb772894925824" },
                { "fur", "105666a324b9287bbafdb39c06960ca6812a24db325d9101f2a4add0ec84d9101fbf730d55e336a3bceb1819527cf16855d6cf76c55ae2fb46bfbdad5af88244" },
                { "fy-NL", "366585126337c0d2a4eb3702fc4773d3771fc918da268d5959fa8fcb57a90d4d9584ba8719678c13b896bec33e3b2ee30d7de2bbaf1c1486fe24102a8c9f1ce4" },
                { "ga-IE", "c6f1b9b5752c27b54f012229a26a66c8b5b9ed3847fdaeca768d8521b39720fd21073dcdcbb717cf6b3f106f30da2753bd9819d48069621cb11273dd8016fc46" },
                { "gd", "c70430dc88ac2640880933dec57023235e69505b511909c6c94987f4523d9ed04eae95ae871ffd3efd20e67a7514dcf15be41f7e5f7708bf1a95833228174a2f" },
                { "gl", "3bf1eb6a8e0366e478e445a52462b91bfc7ba0c6610b2a2f4fbf17623cea8d6b982a3f02d661f8a65b5328dbe7162544f9ad8968817b969a3d9c858c5a18ed8a" },
                { "gn", "2ff07ffe1dc1753413356654ccaf5f50eb5b4993f4acc31f112630db9d291ee273a8d4c3c722d01256772cdf4c0044549ddac609d3664218d10cf17694af4bac" },
                { "gu-IN", "6377901e31586d09bcc30a2248675c7574f4648df57523e042fa16ed3c3bad070192bbd188db50cd23a8ee9189027c75d77a7163d05308bae189c53ed0875305" },
                { "he", "67293019d395e6fcc5f89b14ca7e8a058129b0b8f9f6b7d3aeb0685d0de92881ee6c817ba38d348e88b913e19e7802d2107325ee8a32f6274a8b4338fc0b33b5" },
                { "hi-IN", "82760dc44fa03b387c9eecc51ba9f49cb8a73f2b8b3f4d786b60350aafa37f1c642d0b90fb53555a6939abf639f46d97e905eeabb7c08354546f1b7825f4c33e" },
                { "hr", "79fbd6734b1919c463f56997980b79bb94f6c22e56cbdb6fc1187b4d49e7e752c33371323858dd5f8caff664275172424c0bc8ea050f49abb0154656a6c5ada8" },
                { "hsb", "83674feb6c62a4ea97689e36ccf0ce778c48f09225352091adc62e9f9ce1c494a43463139bca707668a29748e72436d3bc5704c9882575c0f170e51bdfb8530a" },
                { "hu", "cbab6ad505aca9267aa649a0a1971ce2e40135d5ff36c1d08dcfc540ef947a18bfe064687673226c6162669b5017f61240fe100d8623365a4f2f3b7eade48834" },
                { "hy-AM", "84e58b61bd9465fe73de5653079e2e0f04ce007ce27cb9d7d1c0304fce28afbe7d19ffd4995cebf2c782d19e3b8e7b5f720ef4563ebad25c94bdea7c215cee65" },
                { "ia", "b9dea6eb289a36a9af2517301a2c79744bb48b0daadfa11683f7a1811fcdfe42275d391ca76b33bb4c999bda1915e6826345d8ccb2b4b9dd8ff76de89377fcdf" },
                { "id", "35569d49a1d80f835b1e113628e37cb6a9d8b8e9cc56c2177d35889fe0e2145400469eaa543c02889425f17db6f1da1ce5cc32fa7cad6f30576ae1f137816cdb" },
                { "is", "eb959919bb0ebeea345525121242dfe169bee5e8117ee5d58a4ed58b05293903543a0bcc7ff3104281ac46dec04cd7043bef2eb62d82d41933a48ae439339597" },
                { "it", "5141cd2091ab0d4d78c8043e47f94492ae9b0f5810f30e02640511c1135eb3618d661d9b55f8b99c9c109e179df28722dcbe73f02b8159436d6134c9bbdb9b95" },
                { "ja", "3b9515a1b072fd8c95e07ebcec931e6ec0296b41715416fcbc8b7aa36feefd74cc5e1f74dfdca773a61701689b7a9f9a1d93af0d67f201be1a3c4dbb24dba81a" },
                { "ka", "79685d4fbddfa86229664a2352fcc3ce0075b500e6894965a3054eae3bba045db5682443116acdaec8e91c07dba0aed54b93394faa24662c0df68be7d42b7456" },
                { "kab", "27acd8f31f1cc9993e89e1dfacaf31d8cf6ab9eed434cf6bf3b38c3785f73c4cfb573c25079cb637fefbf8614bd2b4946918b38959d08ad466ced05beb59d732" },
                { "kk", "3efb7fa2be63ca197c8b8b95dc9213e8218631ff74f9032ac60b39d83e7cfb1c5096b52b9ab44ba2c391f6717f8945e436cafd868e80bd4e735b9f58f74825b6" },
                { "km", "91b16d001a4931d3f8c5bcc3feb1b1ed7690f94be4788673550932ddbd4c4d5f399294fbedacade93b9f5406a9e512fede6692bb8f34f27ad8cb26e4430d6f4d" },
                { "kn", "91a792b922e4b221b1a1d66ad02e84cf841ae5bad3443539315fa7a83c98ab1bce30e48246f4c83811bdb16eedeaf21c51dc65b1c802ac6ef7209c8d1a96548a" },
                { "ko", "518dc6bcf98e2570f71f4ac48b012a56a5d183999ffb569ba82f91170eccd6bd18b3e35b1095d8ed1e95c44541c104024383d40bf605983219ee4c9926ac6e19" },
                { "lij", "b5d11dade26418942a4640942add9c67dbaf4a82afef2b406c871fd07f24c6211a1e057c8d5abd07cd26874e16fbdca49e34442f1c5c71988e1a8184bd8de305" },
                { "lt", "8c5e512e1b4c51b0ed6a4d26a495155e592c3fed64e2f7fcfc88527873fa1955df921b93400951f0cf501cb7fe069d72cbe0c97db3e4053e001d99187d73d6c8" },
                { "lv", "157feb5cb43999817b2dea87fef09cfdacf86783af22ee0a37d43418ead600576118528f47b99214a9220e697cbb1b07d5279908fec4d9a20b11d540a5e38d04" },
                { "mk", "4143b10dcf4c51c2dc916c6974ca2e010d0607e31423af5333d90cc48948c9abf719c6d5a7c8c5fb606a5cc6dfc640447fefbfdb24a2cd44eec8ad753cda3d21" },
                { "mr", "040f08c75261f78e027c0c6d96e4c01dd102209531ab69533c8bcacf54af257e5cd501ad568743d6e03c9331218ea8b2dc7c90303f269b15bfc654ec9fec37b7" },
                { "ms", "e2784ed8e45734932fb07bb1cd97ff68f92429a57fd11b02af558df0415c34839ab4a647acc7f3b5a44a06e94bed7ec31d9664c8f3fec7c33b25c68c83d7b515" },
                { "my", "5673641ca7f71eb5201a04ea06927e4e664d79862853f100451393cb4db2531f020f6c6f62f876b5d7490ce1eaf1a612145c4c58c80e222489b9ebb91fb80acc" },
                { "nb-NO", "8db205cb5a943819494747bcfc1f140bd22a0b61664fc8be52dc130f0fd55f847e268b3946300f77d29ab2623279392ec4f0e2662bc156df3d445fcbd32d25dd" },
                { "ne-NP", "03dd7c3f4ea4e4e6579aa38c80b74d8642a738bebf86f9725ddc3b7832943cb07c3ab1ca3e63335b8e6a46f492a7d72fb93e4b5843b43e41c0bb43a43d8a2c9a" },
                { "nl", "1833cdeaac6777a46bf6dbdb6a5fe8d18f0a06883c571294a6ef00d9372aaa1e617905b3ef9203d9eb8827837223d095fea745df6120e657ffb596d7c34fc884" },
                { "nn-NO", "ba85c33d2dcfe66d443b9288e5ec4d5055a198ddcf15ea0b8bdb1b142fdc88766fcd9f60cdb27232a3640c1c71a115d085fab54a87675f01cf1280a772e2c6d5" },
                { "oc", "80fe2724da634bfdbb6374c8a0ba0c510abeda8df3812b0b5fd8cf3a0908b0a6e68c79dc4ac174ba664ee534032b9d7fa6650c82a5f14615cc855ef0d2d4b7bc" },
                { "pa-IN", "af320c4c98560aaefcf42fea7eb7e927a55d5670c3f688ff9b70ac8643134c17678b2afa53c88d600ee0f99fd93a9fe7beb1e8e49a4a2668088656edf6280d20" },
                { "pl", "aa8c670d6dfb2748b5babee218b58dbb964cd376fe1066a4efb855327272d2849e5de69c75b209248df12f0f15c76cf204b192831075b1b73f272cf7d1e08782" },
                { "pt-BR", "a13c614e4c5d45013b7412afa72b56f80ce466ee6221c66c1ed6111747fe041d21cc0c6b16d664fa0b0bec5cbeef8782dd9f79ab41f33e011d4833c1cc426e7e" },
                { "pt-PT", "ba58d6776a8f6c6f75a540f018f2758d350e3a7a072790ad70e75def4e90b963292e0d75437523e84e74c43b5a91517b753ad30898cd11c3557cf5267bb74243" },
                { "rm", "f4a87500e6ea5549c0d229b1616de1d2b44297dd4029d499d6e136b998ab8bb65cc4587fa910e71bd77b324dddeef9456150bb948309fd03595cf49b4347745e" },
                { "ro", "35603bfdc7da40c854c4e8713fb317fc9fdbb2e5f350a0e12e701007c3d47c28b275bcd19d2e61a42323c1767c06dea998cfc4dbeba5bf7d59dbad203a334dba" },
                { "ru", "fdec842c60e1244e136117588d0cadeb1afc0625e858f8f97bde0ec1cf82732b0068c80f99fb12296dca9443faf9becaf6271a1bfb49c79e85247eb70cb15e06" },
                { "sat", "83ea875303acc39abc0db8a8a5a46102d9253d999561d9891cf8e0a80f8db3d1f06ff216039b69ce3c59d2186ee2c2c89ad6bcec36c66e8283af318e17edc639" },
                { "sc", "12db56cf23344402db1a967cae65efbe5d67a0b544a2723a0e6f63e4ac4e0dac08dd6eee4aac94c0c141dc255e0bd554b05a94adfef6e7145b806c91ad594015" },
                { "sco", "d393a2be0a2f78648297558488bb870a81bdc0a1cf14655d56f1d9cfd248b5731093ca02adcf49720a2cf6cd0cc9d82ca887bdf0f521769de516eaa33bd1f6ca" },
                { "si", "6e2eb9cc897a294d68565b821c3327f71795d6d1d820d0ac51d3167b2efe5d18bf081b40b78e81db0215266c0032e019d13035ba1f1d67389bd10652330891eb" },
                { "sk", "baa8829635471f604858e84c642a27546629c804121ec2c6a0fb18e75fa142d81547bd8fcc93092f9a19b2e79bc8b95cbe0a70dd78e29efc9ae6de7347c7d19e" },
                { "skr", "dadd6822e3dcb9a7528b4f90c07dd411e9f06ae7141dfd804b6ac1d3d60ac599a78dc791d3e5f2643383f57f18ea4bc42a03c58b92a7c58b8400002f6a807c3b" },
                { "sl", "b58ae80c563923bd28090379ca275f8c9851587ccbdaf757b9ecdbe14d4b0adf8362e1df6cbdf7733c080269da19027cdc4a25a9df852432449e6c0862bda174" },
                { "son", "51475ff279c89088de654d4afea26d7506ed24db833d4076b7766a8170ebb87d2c527c89c49adb8ec27616e8a65c9b6ec48704ce96f079380c5c9d91b0c49a55" },
                { "sq", "47106e4369c2180b5b5287445133041a119e7a4501295099075e7c55c7fac8aa4694f675475b7daf9bc239543068501200772542a1b5c511bd7672a3321257f2" },
                { "sr", "70f08425d997a919763618fcc73c9962a2cb7d866f7c61bdc1257ffcf3a87dd76eb952654fb6d22e91c40bb98142e302484db3612e7ca87409f2f65d80eb3c3f" },
                { "sv-SE", "b9e649c6b098ed886e482eb29791e55a331e8b17ea724e84c163a19a310c423095db4bbb8de4abf2bc5c7e72e96ab66278c30286fcee7b2b879f37c2d0faba79" },
                { "szl", "4a370604445d98cde715e15aa8582d0208d098943e70b103ff9818d7f6b8fa26b160d4d78ae99bbed96a4fec692ce555e7048dfbb27b7c47c75a1333ee7ff45c" },
                { "ta", "e27852237e188708d33ef4ecc49fc4d0a41f728f4941c74b68a2bfe1aa8b70605f7c4df33d4964c2d1eae189cb5901524c973d5c80fbeae2b81198774d1be476" },
                { "te", "646761556e7f1cfac61d25da61e801078654116e9bf63526daa50a2742bd3d197f054255f9732be5fddf9f5f6e969114d82545e0735b649237859fa209abaa02" },
                { "tg", "b8653eee1526e3708113974b2919286e0c45d13e68b62af6e9a9bc5ed2bd53fb023be91980b34d54234db69dfe04be65da74cfedd1f0c32304d673979f97a490" },
                { "th", "d2e6577f460b61a4b25682a3b7497f2b825ad0954bf1ba0eb96282e696d8c19952ae2f353afb1c20973ba50541542328c5265cd9852c133ca252c02d3c395eba" },
                { "tl", "ff0fa355111f1a89cb233817646cedc3aecf8f891a18a71371fa1f970516957c66fa70323f1b507c306bafb718cb12f3536864cba5eff198a450fd804004895c" },
                { "tr", "0176388d42064ae0d4d2e3b70c623b0b676c72477be36ed577bf74cc4e849efb048be3569ef2b791e8d1dcd96728852e672d782a012f85bbabfd4c12af9738e0" },
                { "trs", "9528e2f32514ed7ae90660f19d9aa5a951cef1bb6a956772b7b12f00f197c0b43dd25a08b29a33ac595fcbc865bd361ef391b10bd95cc7e53b388b3a4b28c9ab" },
                { "uk", "79de463a07e466089c27f7341ecfab49c280ed5611d7c1fa82d9beff42a008782f05918ebdcb05b208cab539770c7042b2461744358a4d05454deb836524d203" },
                { "ur", "17f393ef898af504c869aaaaf349dbbfe46ece7230acf7104e63b0ac30a97e590dfc3fa5a45bfa515a3121cc059b714002a9aea85047cbab1f50b2033b5ca459" },
                { "uz", "2c530e997ca6e4014ad4c3eacd23c15dc1da2f0298fc093df751a01f5cae9c5c86bcb92b5c48a97d591fdd5c52f427ce7ce03dadddb35dda4e4c176ac84edcd4" },
                { "vi", "a8f4f19f516e80186d5d46ffb142695024b5659e059c489f344937e0d57b86295f1d02594de6710a44e984f0d44ef6a3767b7d2e6aabb43c5b0ec4e796bf0eae" },
                { "xh", "49a07e30772849da893a30bab03ba0be72e6ab11110a26a2a8c0f661e05ca63726e39f177bd11a1c09878265774fbf951c37157b3a22eab60c8fff4969f39089" },
                { "zh-CN", "71aa79ccc8a766a730ab02299cb89d68ca902ea9f4f712f0976ffdb12a6ba715f575e55456d99b3cb75672871760b61e9fb6a72acb2b0facb382cfb5871bc761" },
                { "zh-TW", "6b582e304c904304b25fc0b62b6e2fdf37f0e47e78829c0ec9977c423f8ccad673e4d7bb085675aafcf8dcfc351660b63e264f85c6bdffa678afed1358598051" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "87b85910f17748000746f15f7a2a190384a0303471e3238f2dc316c498203f66d2a6204470a29d0792b537dc82577090832ae73bb50dbb6ae879857e5785ae8d" },
                { "af", "63b0fd2011cb76a830a31299a430fe5c899a4434dd149aa6cb20946f055932dfd3f099be560fd0f714f61af1b731f0a32883eb5d6150dae15234e3fdf599c6f4" },
                { "an", "828e58f32f346cba53a0b5e580eb9618d8b478f84865d0a151a94ccf84c5d08c9cc827fa3ec410ea01ce14dfb1c7a20ddc4ae633eedeab5d4e4671bf994c71aa" },
                { "ar", "5233547665438db94bf8f8f59d9dd9f2134d68f68cd7eab97200fd0b4ba34d6f29b7ae242313637076beeb3dcef99132651878fa29fe3a162d2074f50b66c548" },
                { "ast", "5a50f24011d784329967a5d6925fa355c6aade265e97e291b21d56dcc5318a9b1670af5e908a358665e4bf1d101057ccbd60dca9e37bb6c1be2586c8dc1971e4" },
                { "az", "6dec174e5cb0a34b16d675ab2dcdb92af04126a2de3b9d8077824df57492ecc5d997bd05276a263d42210816efbb41b593050b2577585c010948c39f6d2d8759" },
                { "be", "29588e8f3f8a457210322db45a738fa324eca7f7e18581bd7709ba081cc48ad5276cc9dd9252a42201350bb1c550a54b341a2d43b73aaece369800b52d42ca54" },
                { "bg", "fdbf002c1662ee8770dd3b43fa234f496c3795bd90560a55c48013a802c4a920dff3359a2fec1f5a2544ffc0e5de5d142e68557ba316be6d75a1b4de4b4c3195" },
                { "bn", "e67fa104ab776db379b44095ddde361e54a510d9bd0a2e4631ef74f222ded3312a21ab75711a0f44b238eb583fc674699dc0c768278fb088307922509555a123" },
                { "br", "b029ff3e6b9baf190eee8cbb9e866065cfc30421667445584923d627c710b67639a653e7456d4681297a3ce3fe160ad5c62546e625803f07f5e16c63b0134456" },
                { "bs", "c92f33d6d2fbdcce4c52c901a43af27218b8fa1b5a692321e8adb6cacbc89535d08a007053f93804fdeb93586bc8fc06d7d31cff9bace993a2c10d8ec983ca39" },
                { "ca", "a375c34fb6dfc7895d117a70ee0746efde78eab89601efb5a3fd47f87daddee3dbeb73b62b6f1d65be3a853aab28e1d71c082569d9f2af44c4055ef80bd0174a" },
                { "cak", "71e90c6a766c3a3bbb53e98eca9f201dae609713b6a36428968cc67c115f027f18caf17357baf9ce82f129e8f03d2b1b11e9a35d8fe341da28d989f8f5d74354" },
                { "cs", "710a3c7d64eb76ea0b63d45031b27defeaef7b35de3298932323f38f6433fb0d7126d9a020235a68e3a4c1882f955c2b502cc17d9dd204012d7328154537d129" },
                { "cy", "4685f93ab66cd78a24b47158ceb8ef14fdd74384e8be79b0ff5fc75ff92276b9dfba4c7383a92165a4af37f767b0e446b31fc095569ae296c9d49ea6ab6f2e5a" },
                { "da", "e79626718a366f314a6efeb4a764b2fc2817bcba65b1ce2677ed7809a19ab54f2ff2c22e3bc08e2e188fddd09d56a1e54f1606b37aafa7421abb92417280357e" },
                { "de", "d1d6b65b5d15ac2d3523e4a7b46b0c8967c835ebf27c5f244539417b157f4442d0ab0af49755920201befb876fcc8def4f053713d00dd4eaeff4a8c1cb7e5c67" },
                { "dsb", "cafdd9b20a40bdf0e1c1eaa06d9e6c1e0eea30f9c5bbc59bf9c4e30a353f383f97996bcff22d7080462db25bea4e27c60c7e3ea442d61713f7254dfe4247e2a6" },
                { "el", "83b1e704c11decd09738fffa5c39a3832dcddde24927b612472c3547590ba34c24e58c8ab92c3cc4ea6a81d6f9d5ec0afc259b300e869c29071e940317e9c321" },
                { "en-CA", "845cc972b82807c31e6bd42da5d223fb0a4410280e7a5b0cde9219e701e1a9c4e3863239d6cebab7a5183bda91d06fefec2b16abce86a50c602aea191fe8d7c5" },
                { "en-GB", "9b26283a0816a4c787f03ee079ee06e420e6c6984da046f67d3d30a3a174ee39ffcf1c8f742e1123589854c18e5ec46c5da9ceb5a69f75de83414bb90d6bbe22" },
                { "en-US", "11b951d4c6a88789840e7375296d06c47a7618f475a46549e3562ed07e9cbbf612a79dca125d5abf635f43f12c937b4cda2eb44011f2eb8b539e1f9920aa706f" },
                { "eo", "2d5d20f1262f111f2deb069486a2c5cda18fc3e8d27bb6029ac5c16e2af75e3c32ba746300e2e06d37583fcacf07dcd1b5bde1c306d013a1af3aaed0782d5118" },
                { "es-AR", "c17103e10d9f9e55095012b107a48c4ed1e66305fc7dc31738af9ab6407f0c304a2b160310debed05c0553342de9f918ed9d925d36c03b20cae3ee172ee4e817" },
                { "es-CL", "cbbc0f55f4f3a7a21bbb532feb15f045823c5ae27edf24d1794d5a0bc0eb12775866c269cd85d38880f0a95a85736fdfd8f6ed1f9a92d9630d275b3c929fa927" },
                { "es-ES", "b9a40c9049cc693ccf2123556f81f3548361d98801398285b1916fc25842c52f080a0283cdb31c4555d45e9c13ca233bf310cbdcbdea10ebfe5484dedd6d5dc2" },
                { "es-MX", "d9f12ff567a891d6d7793188a2d325795bac661a0245c58f6d2a91e0b9a5b625823197031542fe94345c27dd33637010bb18899bdbbade44fda2e84479c98792" },
                { "et", "562467cb8c044d0c0ae636234bc844579f3ac6532f0cf36205b00db8e458005a0c34c8c074c461080936ef26eaa4be024975d7ad7273e7d37e029cde00e86e5b" },
                { "eu", "cf0358f22974b9a6ffe25c88675bd8deef1fd7bdb7f8cb317add3730cc312cd346d29ab0bcde948dbac1374879ddf38b0a736edd626b0bc1fc30d8713dc720e9" },
                { "fa", "d7778fd42da2659978a9730c9f6a8a08785dc07da2e7535a0500ba33e1f5571263504bc96a0a5768c069b0242d5259f15589fe8deea96b8f142d382cd731d182" },
                { "ff", "faeb00ad414f81af3047633aa3a5600dc3f7558f7842dfa0e43bce3d10f4ac50cede7523c9ba9cca677ea95bbe0d1f0b61aad381730197f8c2865b137bd90f14" },
                { "fi", "78b0bc3696c5598360d19a34f05c65bd5ed257feeae2f3e4af088ff98f25bc9bf11c69e01fb338e9881baee154b0f84049b8e8c01f879dd7de85f9e6c6f34507" },
                { "fr", "63c5c98e4213fe4cf795e7232fc5ff82ac5f8983089b7454f5b7db85af572123ea4cc25b75bdb7ebe4ada5bdccd9d5b3accb04492783aa57d67cfaa8e782ef05" },
                { "fur", "65c6d1864682e1de08ecbb1ccf71dd1de9c427bf92b50e2db19473da5bac39fd7facb3c5712446e0f200a0570b764c7719b6bd57987f59dc27ca7800c18f0416" },
                { "fy-NL", "7bb7bb019319bdb524e7660a46e95e26cd58eda2b5f4088c9ebf233ad8766d8e69041dbebcd16b1c3b09d13fea177980fb92d0b176bfa5ee708bfb145b939267" },
                { "ga-IE", "2da29e8cba5fc8a6e4aaef7a81ce401ae18c02853986fb63efd0373f72af08a4f1dcaf864f43d6d633c86d77740d8156962f5feb87a204f56134f65f99105e15" },
                { "gd", "32a8568e0228bcb9604e3558015c204b2010f0140f87652fceb1fa5be7da9acce0a780b8e51bee5602538275b4d38b70ce3c8d212701bc85d431a737cf0b28f6" },
                { "gl", "bd8df1624387b76985e44738ad7163751cda7419d221de403a31683707dd2180b0cd576fa952e9f6f1b3cfeae0f9964e7caeeb6ad3e6fa50d9dea9a12b8ca422" },
                { "gn", "3fb23acc8af9db1bc8edc6ca937835fd7c6b92178cad322a06e65e62e175cd617b897de9c08f4e6988ecec3c00ca4376bcb4fb18e3d3bbd29b23a92cd213be1f" },
                { "gu-IN", "3dde5c90b7611a4f846e0ab8bc967a011a2467c6ca34aaf3801ab8f58231b35f269958c1e66dd7e2b1bc7b5f91499bb16071fed3ad66446b0cea1c6db653cbb2" },
                { "he", "ad6bb21c12767afea88dcc59efe3351c544232d6251130454fde33720d480b97213cfdcaa20d2784ebf1a256e2e605057d0946c0ab957e18b3702c0fcd9aa378" },
                { "hi-IN", "f746bdc22e48d4f3df328a3eaf81efd2653250d2aa19a6447b713a2f1ee9d633eec12f437606e7cdd67d9c1e95c1aa2f1258e837071a0433be7df73cd8b8e0b6" },
                { "hr", "3edfbd3c9098544ec5648366f5a6b9a25287998aec5259f75a608b0cd0cb442be342e15e235e1f6d26710119f48948a3d489aa266360aa411add2ce65bfe6fdc" },
                { "hsb", "12838636812fd11838ad45306e835d7ee49c813d2c12992256f5ea3631a51a68f1e72ee9faf986ccfb99c6ea736281c87c0b61875cb8a1f537647c0b52eb07f3" },
                { "hu", "08f738f20de7994debb14a81c02f7b68dafaa29a34913da5214c300d58f556bcdaec4d725e1437d0095b71603d4eda16a5adc81ec39a5f0a8ef4be3917f36b3d" },
                { "hy-AM", "21618dcbd51ed463452aab8716039bae2ccc79e14ac9233b037c5f8879fe320319d33102f4430f1c70bc44ab196ed65c2a1c1f99ea23986dce01fbec5711cf77" },
                { "ia", "0d1a1e7691c4c9ba67b7bc9dcef4d8f2920cff272b26686e8b18e06b5450ea51b52524bceb979cff3671619ddb3729a3b2be7bb52776a419d6ed9ee0db920e52" },
                { "id", "480ddcd09be21caeb6114e39908342c4e86c8eb8eb96b88dfba93110c514cbada303a94443bea5be3307074539cb2be23cc307364bfcfb14e6d9061ed3a2c6df" },
                { "is", "5d3dd7c36449526d566164fe899cdac1d1de78c9cc740d9bee3895b767363963517eaa98243bfa0e1b60cea835705fe4d54c26c04619c3fb099130590288df14" },
                { "it", "44685a4bec42197469c92fdb9237defc0a092ce9dbb80ffbdbea9fea37d27312a1105684f4b99fce5c73b87d57a722b7f3030575c6d920d24a1f0033957aeff0" },
                { "ja", "35b58b7e8cef2609af5bd4bcf6a67901de47ca166f8ff23f97406c1455c6ae0a1c0ed15dfc885d201dc1eadc033309ec76d4230380956a57f8b9a7bd320281fc" },
                { "ka", "a7fa69ddb4ee1129e9b88c0321e44402c65e518ae7b495040190e27fcd3d6c2e7e8d853c1f22becbd7cd6819b37e03a71317bca4fde9ef7bdabbdf076c877339" },
                { "kab", "600195ce22015d4181ac95699221170047ae21e3b07bdb6cb318e25be6259d6e9f72ea8fc0999cc5ed229b47a8ec191ce1cdcef3290c381b088e2748569f969f" },
                { "kk", "c1fddcffcc17ce84f9da6d87d949b6e6d011dabc837c7c8ed4020d4889fd1d48de45baeedc3021e8b2a2c6f4609bdd16fe3915208c9e932cab15e622848407ea" },
                { "km", "a3fd0e4b90ac16d112334018da5959966010e788d7633d43082a952a4b42b54ca2f916524a0776e9efb118095b75316e332f3cb4b39f0a2ecd63cb8feeb75656" },
                { "kn", "65a901685280c49ebae8f0edac1c6a7aa6ea23f742fcd1a0cccf8e29a60bad594f07ea97521623e0902af841e3799b2ef60fa8f6fd8414f3858f98a3ffdbc32b" },
                { "ko", "1f00e834dc289c59a9e2301930e439c9c4983fd6d353da62a5ac1d7f5b382973ae13049e5def3c271e9cce254dbc552c1d591373897f70efa0540c792c056f0e" },
                { "lij", "9615495891f846c1c9467146e4613a750ab51f12a48d618b4c3ecd191f7f05dc47c1d7777326533feafd3c78c336f8d66aa6478a9befa264c1573210b294d952" },
                { "lt", "5ae3b06f519a952e98a50ee62d2709d447d7c089cedf924f1cbcae2ff40d96ce7c644e11655f5a3c7ed30913466ab0afb0bbaefc4b7688209a2c9ff0b774d68a" },
                { "lv", "6b7f7b6fef2f96730f9df6d20ad0a4f3eabc0169d0db5b2cac765008727f81ae3bf10979f5d23c185944b4821dc465f0311632ab078e6cb141dc8714d4d45098" },
                { "mk", "07eca09a96e131a728e9d4f1df50f225aea395a4a91cf63dc0107e02354bf9fce8e2c1b2289a89ad1b040b5ff5e37985facabcc91ea8494bd26d836ea035b147" },
                { "mr", "846b58b772df11973aed886d8d9b1b7d04866d2a91fae3692a3b9875743ec6f69600008b734f9ee5bd1993b59109ebce9637584d6f32a4c4a5bd14d15da8671c" },
                { "ms", "bc5a44aeaf94fcd144820ccd4b4a700bb984556fb82b17a2dcc3879c253c4ae5601312cfb2ef4209f246737b4cc689c2e064b1abdca757f92e7f503564d22ab7" },
                { "my", "a6e2b36e0a1f5aa6aed7b0e065bf5aaa6eb9a1f443046aec878b1d88a624ebf14f4a78253ca27e874a6c00c1d3d5fdb8ad3d8696b00cdebf53d19705a4c46045" },
                { "nb-NO", "734d3659fd64987929a08e46fcb309bd97e0c63d4782117f4cb6907e65fb0334a26bfa66d55aaf7c3d5abf24584f5e492d9811921ac4501e82cf863fba365739" },
                { "ne-NP", "3485d87949513d4ea7309ed6e8dd898499b6661e46b5af52d7f13f9124c5f91285da2858e1e0799d54db6142e171ddca81f7929874ba6aeebc29c0f5c737bca3" },
                { "nl", "cdf868ff4617692bd7287bdb5b8c59ffe3b2ee99af1ab071b5e4fabd1f5d3c29353f835130d775f69417a4139eb8f2b7d701261cc279a71fcb91e52d8b6650ea" },
                { "nn-NO", "29e05c45600aeaf27120f81f1af572a00f2323335de1509d2a6743b38ec4700ba036bd7728963216406ab7ec9d9ad90580d778209a0622508fc73cc4e89a0d56" },
                { "oc", "a0c33a23d20dfec2f103036ee94503c8abb14c0fb4facf9632cf92694720a1d2a6e5d5157bbf8183b1806d71f0296ebe50340decb68e2a497f6d19b2fe5b5c33" },
                { "pa-IN", "ce9ad4557c79cb058217c13d8fd60dd152851abe66b402fbf74256c83e438cf5c5082b24301b0795e2753f4ef6916e8539927201a6bc6b2f6b66ec37956b04f6" },
                { "pl", "bae6f47ec2a110c84d19c00398fdf7e63de4e4cf2c233238e68fdb4b5ff7e9c0547b12ef366acd47bee18b371f7f7f6fe7d0c5ddd76655582a4cfdc491b4c90e" },
                { "pt-BR", "55fb1c410fd3b392c9773f39908a4b4a332ce17cd0903608a45f869bf41bcc220204cb7f2eb96a53af72dacff4e72ab748e718ffda12abf4417c6170243bb835" },
                { "pt-PT", "7168528522576dfe231361c01001c0e87611f1c9adab66f51263278982572bc3a4cbfd4f746b274f6b9abd3ddb3fc7aab782faf7fe204fd82a0db1d44e7a7df7" },
                { "rm", "060d069b756dab42d05685813a1f917259430dc42e2afb8dd098b857829ce739265865f9e5d93499bdc9c6f39eb80db8abe83913312255f2099d248ff11aec9c" },
                { "ro", "c732af5359ded6bfef9229021668bcad29c8747d4a37405c91c24bdd1a4c8ffc55496700f5111ba10fcf9922c81edda8f7652312e7fc533ff483e2972d37fdcb" },
                { "ru", "f5c357db63aea50a16b4a0df05936e396ba9eb8e40c7ac7e2506b9b0a54b441ed35348041fdd1b3cf6b3343870f8e2a5391a7bb9fe0f29aa36e1e8e787de4e0f" },
                { "sat", "fab567b2734c19c233b4c5ea68f6db96f36c04f36244303255dae7d103df720e58457463210b3f4e3e972c743cb4683b0252846e47cf46caf60f49393d24830a" },
                { "sc", "e6a29ecad94c0dc87509317f2bc7a22373dee5c440844e3ff0fd5ea23c1296df2da9418f649220d633d15cedbc6e412f0e276365ce8532e25c9796916a0199b8" },
                { "sco", "070f1405040d85f7728f20d040ed6e90ef93f387cdd9f5578adb7ca3437dfff6d048dd3adce98d91d908bc2275ad1cd8781a84fd4d868466770a459a9c58ecff" },
                { "si", "0a8c3a71335d4ef5a3c8f242e6f7de21a710b50bcc3c93edfdcc8e56fee83d38428cfe5fdc22047d3f841a3ed32f8ada8d2e070f413ae9c5ab8ff2292c7f2b14" },
                { "sk", "d7a1444cdff074a33ab190150f693d300224384daafc8119645b7651e76e16ef104cdc2e998efe91a242a311f3fda05a9ef05ea633aabf40f9deaacf0d25260a" },
                { "skr", "199a92b18592fc3337203a17515f94268e777e8fc59e6b327c75be8a42b4a9241a0e2de17e176b0ec2c6ddc8f788d8db323722c59a7952fee16bfb1f1d0806ab" },
                { "sl", "34be4105091a6a41f58208da3921817af855df6975b33a9d31aa9d80ece113f70392174af6a664203a0eb25af0cb5545e55f41166dfb7a476c19f548d4176b0e" },
                { "son", "b9bf979ad4418c24040bff9d0f363e4be3bcfaac9c50da371c0bb7c96235013bfe7956985716f0f29e51dfd52f90616c99e457ede0f0c72d21d121600bb66efa" },
                { "sq", "6abf025bb70d8111fc02cec8e358ac5668a6643f5503c575f4a6f52f458b8c32772b28305b50878fc41ddf81520e22f04b72e5d04540d89822d30fb74c938432" },
                { "sr", "0cd6d164d7017c30fb4faec5434001afb2d2d436e4e98d926c94616db44003efaefdcb179015b7e316a9ae006fade2d7ca84782c11a8630f489c5769876b5800" },
                { "sv-SE", "0689116696a43dc902277c650fd97ab7beadb06d827172313cd7794b45df51daed0264a8810223aa97c3a4268c4755a4181165b3b5067d6f293b535f79e75793" },
                { "szl", "044df97f96e69e9129bfda80fb7595b9e38c9a9be434a500322bbe4a0bfe6e3a95d186587ca922d2f042403379e2a3849bea404a48ee25df41682bb8b8bfb05a" },
                { "ta", "5877b04dd52cdc8f3f8ddd3a00b11f7ca88ed8855cdf16fce69a04a21f605a9d8f12d3faa84e331296c33e0ae259a75242509f44f48e08fe262a375bc97d2cc3" },
                { "te", "3c0888ebe488549cb0af3e4e9106a660308aac62f80b7c9c54ce03bc620b8ed6b2b455c240bacc548317ce83b5fe436a02371a218ea2b35054885091411d94d7" },
                { "tg", "3b23c154772f4cb9dfdbdaed1c3bb8be63f94681f9f563588ce22ff3a2b2e20e88a3d14c3133beb2578f8d2371c5782bf9e018927dd9e989a2de7a267519b773" },
                { "th", "2eca1416125b01bd1044e84b01bfcaaf615f05893abade33e13789eb0a9b214271f286c9377f3e27fa5fdcd289de8548b29fa406d26a7296949ccbcd00a70ab4" },
                { "tl", "ab03ff0b30905c4951261ef9af31f2165a2b28c959fbf0607ed69b74793a7ec549fea7d8d3c01a259acee99b0a6b9dfcea4cd0f2bed04ceae1ab0f9e8815e00a" },
                { "tr", "f8dd9191421c8f256ef8a6a65022b476a63f2898a81643a893eeab6067f3096aab1dded6ef7f9aa36531c6a3d8f499496258f980e269d89018f5a513e46dfb09" },
                { "trs", "4addb7194fa0ef8293bad302166659fcb650b660abece5f1e9de1532e63628e4d34df0676d2cda415c5fc0e42fc8a3c2cda5934651140a1d7ea360df17efe522" },
                { "uk", "3899b6c8d5df224826285ce663614f1d96b6873ec6951c6726a76de39b3f27d732efc7aa390df48d9a3891940b0a7f83d3bdda50f6032126300f550b0d743089" },
                { "ur", "e24267907a7b38420f4a060eb77732a19e54119aa58268fd207abb388b29402d9eed899b990b5b7638491e7f5dcaddc5258fc84a6f0beca73a9a00800b2797b5" },
                { "uz", "5832b3629efbd4d7a6f2001938b1180076117f7aa605df111a655d88caf05fbccf6a6477f5c2b1aa80bdd9dd3cf6d262acd372a1d3e72977581903c12a9d72ca" },
                { "vi", "22239ee9e285c691eeb9baf5774ff494e964dc29aa5a32d1173d2f4b24b5a146f81b90c24b55a9c56bd630554b5afa80fe514d0288604d640f8951caa5865297" },
                { "xh", "045f61eb0d0484b194cd18f0bf26151d17de7e048cb6293823cab71001a485353ca34c0877bd5e4cf8e457ad5b472eb8a7ad180da12d2d4d8ef5f6529c87c918" },
                { "zh-CN", "234a94492548f250065db1a9d344ca4b6c66b9a04e80f2f5382848d0a9ca434d265baf25f0a29988b962967e69052b6329ee93bce7c25668b24750ba0a431117" },
                { "zh-TW", "4582fba1e94b4ff788c41e289b392948cfa068e90e10a5b5ae01b9dcc23650904768fad3748fd87b34b8f8ea65c1c2fd709323f95e656b1d160ac19fc1b395c6" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return [.. sums];
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
