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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.11.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "3afc379d5c752f7c152ddca493cb644fb7375ae1b027598634ed7c9dadba882eaac65a4452d74ec44a011e5b0653f13d631ce86b5806ca7f3890185f598a5fa2" },
                { "ar", "427ef82d5c5b3275675a5f77c3e220f2720052290a689b09a6a8e5877b4bb7bf74e2eacb9ba445f5f2ab3be9dab7617656cf0d95dd69847018727a2460baede2" },
                { "ast", "213949f712e868d2d70280f2c2c35f16c284b35d2b7f6ae6557c0e7f5e59098b52b9c02c799480b17e09bae05b010f4a763521a2e659c82dd90a52e265720285" },
                { "be", "7ae559cc3c2b5a5ebbf0d33c5d313a3e9808a5899def616b33f2f9998cd60fde2d30085b3da7c6178203d1d900509b720cd3edf5417df63ea226c1dbd9744f79" },
                { "bg", "e17441edf14dd3a1951dba6f71abf30ad3d99955ae6b0a1829a80d6b10e1abf7281f039340e300cbe3bbf252da07a41ea9ec4bde2e549739ab04fd4f4fd9520c" },
                { "br", "1a99c30cf8fd52fc109dc052308e4667cb018598b5839b1761cd719513b7742865105f0a208674e61a44976931b749f9252ecf759251ab8c3317ea79600b71e3" },
                { "ca", "10c7592bd224fa176459dc3c5b511ab7e0b5220f04a5b1480bf28c1d5219a51d792f30502ffbc344b173a6bdd7a73d9db531573a0fc40ac87ae978df2e647cf8" },
                { "cak", "391e6648a1f1f86eb3683a730df17914e49eb8b5a11f7daa34118b10a0fa1389e96ddd2de7c8639c1287fa11096705103567853dfaed4a95a1d4701ac4ab5aca" },
                { "cs", "7f4909d205e73227d6f7fe41d8252fb1dfa3d6a58cdc31f72a833d45597d5b263727784c43f7e4a7bbbb308e137c96e973a1063cfa9d0909de808190a2c7f048" },
                { "cy", "24044f723b9e7417e075dd8e4b619bf692889f12c8dc694428dfbcb1802e5989405400278197e618380a296bd767ddd03ad80f19986a8ebbb74578fb685dabf6" },
                { "da", "191329bb61e73de7748864f9c74850c007e3514dae8ffa140d551b3f2e1e9a545fa1292d2e7151010c013a60d5267f0ceaad575d40814f0b26c8c9688e791ef0" },
                { "de", "92ffdc0d24afdea510b9b125c1715665b146b37de1f930b2914ec14da6c47a17e791e5cdec1a01d6e1518ddf79ec3b1a1d424dc067aa07d078831dfab5c55a61" },
                { "dsb", "05f3fec1e34a33f2869e33a125e24ef11dc21b89f9fbbad8a176d514e5708040e2d4e3afc523e39e4a530c617e372668af916ee88094edb74837058ed96bacd9" },
                { "el", "05fd58af73ccc855d1afed7ca94ac16dafddcec95f5a0554f1044c4cd2d91945e5cba3a4f811e429717ab02570c5f94dd10f6a61418f7a3f3ca55d91ab2a36d9" },
                { "en-CA", "92be8ccfb909bc38cf87ee517fefb9c97c9c926af5d6b0353d54b52b82456a16954e3287be13396c6473bb8c14ba17f5c09970adfe9340b6e2c73d2e5a2c6336" },
                { "en-GB", "a32978b34d8b0adb3ce7e3376eb9bc56b6edede4f84879c003fae30515af351d32474d4aa981831d216e8c555c4bf10b773de1e594d5394dd44b84062e621538" },
                { "en-US", "4a90666ce948e1bef8ba64bdab8e536bbd7c0a27bdd2284322dad286231e4d1ff97dc0825d983aed35cdd5d2e0af3a59b8d0fcb4364ee7646b3a68836182dc65" },
                { "es-AR", "27b737b926bc6cddee976a2641b6a5219c36853f93bebb86757476f1c650cbc1a193df7356a7a9720db3d9cebd3adb55f333f40306e97d5cf4be2b8685e43ba3" },
                { "es-ES", "2369ac10b7ecf804d8b27b57d2eab2ea278defe7dabca16461912d41e1e0cc76ff879a34e9127d48b375543f368d780f8ce57c7e632eae71a1b5277d88afcbfb" },
                { "es-MX", "d6b6f8da50b189afcec2785fd08edb8f6de349a35d6f43473c9de3ea662bf581e791e7ab21eec17527dc39656dd65367fd3b6f6b000e288330e754c9ed4a62f6" },
                { "et", "33d98c34eae53cb9defa12ad577a24e9725ff563cf3a7fef44977f987b7163936b7bcdcd1354544d69f302e7c538e81f426f8acc69b7fffe2018a73bf7073ff6" },
                { "eu", "1fef918f488b1cefe0cbdd654e8d66e4b249e8bc77f0913b174f5eea8d0a043567309dd2eadc65565fd0e50358ea8369bb55a4db7254ff3b4aa6a4aaddffb01c" },
                { "fi", "f22adc088ea34de226ca6a654db0bdbd5bfdf46ebc087e34d1f6e170dbfb6466c325d7e7ff18637cea5f2ede2dcc2281ed74139fa1c07ee58a6d71adf9194fea" },
                { "fr", "816a04177df4b67bd124ffbcdae427a713493355a0e88eac7236569e6c460aec36b29f4f661e04388d47fbe78c0bf31f80ccef4b2fc644d619572a44266dd03b" },
                { "fy-NL", "a7622810b73e1c5adf1d1a5ec694141b39509e1c15c043c858ceb56357ae99b887264d3bb482cb020eeaf838a219ef4e3ca52bb31d45ae5d308e281fc5073816" },
                { "ga-IE", "6424a9704a56b46e9c53fa0535a30f88889f00110099111ff61eb2c4220dd558af2b20f24fff163a39be395969e0be0edc1eb197f1f96651dd70f87eb67d9cf5" },
                { "gd", "3e74b7f1047401279a76e4ab93b5c1bde637da8769e1691f4256c0b4f16d063461bc444f7cfb6d80ed9c64ad7953915fd283ddcf2f74452cfd812a695ee28baa" },
                { "gl", "f3728776b6deed1d5d7ead5ac814ecf3b6c5ca24c0a73a0ef0e002934468aadf4be78e686bd98b74d752c7424489b19e82f132c238063e9e0d0361b6f3f51f45" },
                { "he", "e243aa5551a54e132013dbbd14b36d3dd3068a52d9ed42569589176f62c94f26591f139b5a763ca92b8baea43b092d977d4d0890649597fcdc3dd34e5f597675" },
                { "hr", "957bba14f1c487bc63fdae2d3125cad8ce82ef5aca27f7dd7e9fbb9831e0d3cf037d55dc832db06dd147ff013077f8e0067eccd43f85758edfefdce256e84574" },
                { "hsb", "11dc90c108f810220d72bfed7579f82aef54aee9639aa98a87949d9ec8881af650df52534a83aaf6ec3b7d8ae21d5c6255ad776b37d594bcaa17be2a7f97e23c" },
                { "hu", "b4772e85c919e0d7627496a8ba51676c5eaf6cb96427bea505afea373256a1e29e8e7d68d0809b3f7274b061ed8396fcb98c5f9e0a61570af449d7ab8f3fd9cf" },
                { "hy-AM", "c1c8ab4d06c13ba0f54352c9c17f34064ecca455ec72f986ee8ab8c9eabfb1bae6fd39d2b1c2d8ec24930c17439a4ef91e2ccee7d20e47033405a49708186d70" },
                { "id", "53e85104e44779e5c0dbd569a6bfdf921f15b52e5f70e85a81567756c6f9ed7dc371b23801275307e091720aadeaae5838979f2de33ba1b28f2c38077bd3f9d1" },
                { "is", "1a33075e95d5954f98ed73671cb9ba48fba7eb8fede82787eb1a3a28bb6d3e7c8b31a724b4fc9be55a14aa3ac59f7fddcd2797a1e6e2bd8c725904e527fbf112" },
                { "it", "ecf9076bbd290a0bab1e5119b161b3ec9a65bbd6a7da7ca177c540d8ac0b8b3620fb596f6d80604f43dc83b6a3cfc71da3bdf5ef03c19e543ea029f0d3fd1645" },
                { "ja", "1ba50af081e572c77bb9f9bb08258d236c54ea307cbd3e86f3faac702c199eb05e8e8d16be34f39650317893477f5f1fcb1c5efd0718f1694694c91d99ff3ef8" },
                { "ka", "8fc0509e27b24d49cf88ec96d841fe6f39278576e5bc492edb59fe120f117e51ba8e6b9ed79bcb4609543d88e492cd529d7984f1334af157c9ed1428b7d51304" },
                { "kab", "e257fcc17f97c304ffc7ef5f26730c568a83b6e743bfa8aa6d44f517e2e845d974ed718bb2ece17b667b60620f442b1d2d6a05aa523c5662c836984213a59a87" },
                { "kk", "746955592f7c0c6dc033c60b0be559788214223db6d958225194572f8530c4ac5f431cd952953a8bdbf4e7a3b25c74b5235295801ebb443ead24c1136522d6fd" },
                { "ko", "62f1c1af6357dea74ffb67a7f7e50222511f9aa889479227f9e842cc017e6f0dedddfd71e03cb9cd483cb1c348d17cf92282b1611a6d14c46ed6b400c669db0f" },
                { "lt", "04fc81e01356f12f6ed401fa3ed82fdd4892e5a2e52f317a8dcbd91c1791ee1988e54720ae79eec3469971e7d8bc4b93510ac986026a22bdd115de1663fb8210" },
                { "lv", "b6e447985b3d2bbc27e677cc312469b38f6f9a3bb6be5a4b88828e22af9fb80f0626e62fd97d871f2ef1eb780c449ba6740e9752022f2e7256c96d764a5fcfe0" },
                { "ms", "88c971c7a0935e47dfde705b582fbe244385efcfb0af9ab25a6e62c4331680cdae98e87c7e0c6bf62a27c67ba5e4e3c6df690623ca8cc260f13f3d331552626b" },
                { "nb-NO", "dda37961153eb243469a44847264280ccca47b74002c2c487713333168b2fbf343687eb60e1a9470f15c7d2a4c4f1a0a5c903dd704b6c0aa8ca1c979b7f5e1e2" },
                { "nl", "8bbdabdf433f8fd017f607dc4d1b4c6d59a5cd82ab8fb996b3ea016f3d82d726951fb82bdee3a6b06d9e9af5fdb0c4f82026563d23a546c77547bc51050b51c3" },
                { "nn-NO", "d2f1e1bd1fd7e14720d7f976dafc7ef1ebd7780d78058ff883235478c9d2c827dcba55bd8f1f5410cb65bed5a21d65c94d87e89271032523c7a84559df1fca4a" },
                { "pa-IN", "4a36f26d983a281ff9e421a66cac89039b8c9e4b7326fa0962465ef1b345f8d2ff547908cc2647c2b53cabbd6d0db1cb9aaae2d7465b42a7ad4b6457d3c8b2af" },
                { "pl", "87e830e18ffb4ae99d33b10256d44278abf42f7417147bbc8088b315de5f29f29397bd60b067924185f31f51b623f50f57d929bf39f57c0901a9ac09bf0922a7" },
                { "pt-BR", "a7df370a6472e22dd222e9dbe5889fcb147e3c219ceb4817d9c6ba1b2ce61415a8727cf190b20d268d9b4d8d060ef397d6db68e43c0b3a226d2ca8fc04a6ac4a" },
                { "pt-PT", "b4319b9522e67454770a50dfb6f8b9555c3b521a41fcf22a1b203c1239f73d7cc0b853092f119268762a896c5418782d3c37c0873af974ec032b420014a4bacf" },
                { "rm", "5a5784a7559ed00172124d3ee9b6438795b7a0fcba293336bcd7116fa8822a7d8fb36b4697e9867697a6cfe5776f113a6553a53957d4b165c0d7b56a3fb5b981" },
                { "ro", "2f4671702a73a5f90c87a602bbf82bd385c6c0dc3d6f421c2d2772be30ca8c1338a7323519c7b26af3b381108c5b8ae5da6ea6b188ac2ada4cf5a3b2dce2cd7d" },
                { "ru", "5c6174cd73fa33ab2f180eb34d80bce3afe2901b884cb3c05a78c62f4fcdb405f5a041c5fcbda50a63955b6b5dba19f05c1c25dfea4de84db41bfcb7df688d98" },
                { "sk", "dc88ef884c0019d418bbe63a35db680d085f65ac9b27c3607dfa40dd5e5de3e90a72a9392a5efa68f2652061baecb2acac63f99a2b4d4cb1163c692bfcbdcf33" },
                { "sl", "df935f9a0076b7cfca9143e348655e77af4512caa1aac6be3500a63377644caca185587759cd76eb15923ceef0fa2b3e481c749ac71e506bf8f2c0b5ff3761cc" },
                { "sq", "30d29b09ec47a2773cdd95928f24c4a045227888251b3b6813379a5a87bc3692daf9f4e0e64323b8a86ea25d8e189aa0fea01f7a8065e8b3dca484db24f878f0" },
                { "sr", "b95bbca0f6313c17084e14fddfd45f311742b3fcab077f967278476b6b3f0c32b7baae730769cbca614e20655a68a29f6a2d6ef1c45e3a2e7481d162f11eea60" },
                { "sv-SE", "955300dec8ec6e2098faa0a941a85d35cd523484b1454bd0515b5ea01a91a7a3dfebcf44976c6b039e667c68eea069b5b56f0d77fcaec80f67afa8f0a854a7bf" },
                { "th", "1039c856abfc9524f5e1ad1fa39903486bc5f16235286d7e418b0c40054c07a3307ef7acfb9e2b0cb45f2d213ccc9da88dca7d656d53d11daa9f4fe23424706c" },
                { "tr", "4dcd8f877720ddb631b8b74e0ffc945f79c70f8d909445c839fe2d30275a3134a31315c23fa46e2963fbe638098820c48ed554de1e3d8d9f361f101263a1f736" },
                { "uk", "baa5267121d7991b187c3c465f4c523475a0ffd6931c684fba2aaff88e299fe7a23e0253377e3a1eb170649b0f944e82d1122a98f2059dd596581ae38b6498a6" },
                { "uz", "d9492905498a8e12c6eeaa4b66bd6e37860943ada587d68e000fe3fb581757e075ecf71d8b97bdefd894a01a108593c061ad62499added6dc14ca1d4364d693e" },
                { "vi", "9c86c4fd20aae23b8d3ef0ec1057352a3049d97774d358d1ec4cd6f90c913527d60882e55dfb24fd973967d2b3509c40b4b10a4868fa8f5445398d5dcffc48cc" },
                { "zh-CN", "ba624f95896a9fffa4a7b21f08c1dc144e7fa337f9f717b0d924e9a37d97708743c2d899f72686adea7674062f9bc6cfc76e518e593867380c51ddf39211e1f0" },
                { "zh-TW", "23d79acea1088bb860b87c1b22b56e9fe3a437ad282b0a350fca8bff7896aee56902675cdceb62f27a58c6f19a7450845b7af1dfe3b4d50daff0ebd5126dccbb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.11.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "61451d3c459d2de13b662e3f0f371d416677abb9a6b1e0b22ec65b63b89e8452b587f145c6ad1f99e265ce9cbbf22c17d0c33bb6c8e608df3b90dc7bfe2c0388" },
                { "ar", "815cbfcc680f2d418921a3f9f2cfd0fe63e8f9858a47afbf52ad8478c9c1ab96672e45b78220a256cd91b415ccbb502253e85c65f3b713f38ee7c37b113fd522" },
                { "ast", "867763ce4b8b135d29edc0b1604d6bc6e57dccd5346571f02106e294d33b0997c137c489348bc043cd164cfad22e603343f3e281e786ba32065228c7fe1ff9d6" },
                { "be", "305d7c6cf4b735990798890ed15e788063440dc078d95dd14184315611d79ae7e26fd78732335e1da392ab84e5361dc540eea3dd3fe759701bc679c844f6d393" },
                { "bg", "0c4170ae1ea3ff3e652d323948a6ec589c074c4e0e306bc4459a0821d6cc417d45be8af1104c194bdac7db032fe9920454c35f5825d34c2a0429b89b4d985895" },
                { "br", "4da82dd67f324139cf48ae0573745c78c990c5969defb74fc550de358fa9564bdcb69999d87a391816b36903a773ed94932a18cb44ea51849022667e227bddcc" },
                { "ca", "d5313b350704f0929e019a65ad62468257161d12fed4b45b9cf586c8897818742ccb393b1c1eb799af84d3a84f16e220fcc58c6665e6e88132b04ce75ca9971c" },
                { "cak", "64c15dc87038e9d08b16ef8d126474243db2650eee5066302e43e83f39eb7c94d06c5d2e97db2ba8b3324ac18f7fc37faa198d4e6eac4c96472d365b001f1d8f" },
                { "cs", "f213163320dff2cf04bc0a5b38dea04e2481c7a6e3fba615d50a2950ae64edf1f4a58efcab772f03b25c8cb79c9ca1e7410d1d8ae78c6f699b088b233dc7dcad" },
                { "cy", "28394cc9a366385c4098ad72dae99328cb32b8d20ad466a7d3427ffff782ec906d4444d01bacf22c323d6e596f6efeaf47d2644a0884dc120db65f87e0f3ee88" },
                { "da", "3c01e50f4f6dd314f523d6ba5ee6b64c859ef22eeceb53614fb1d286fd9e4fd558cdb08e137c073a282b51dfb6573833085b31769bda77111658fca7bfcd7fda" },
                { "de", "150b1934fb1d90b904a0706a460e6a1f2092759501c8d3a38810a8657b0cab33c0db6a6a1be93f90cddc23df506312bbf21a39b110cc556acb10d977a1865879" },
                { "dsb", "ae5af1821c136bae457b11a723d95db3f74c946810dc59ca040cc6022818aa3e42f362c17a6662e75c418ed83b704c766113ada55ad30452ade1690bd6818a57" },
                { "el", "de28780030c85bec725671991efd4c608eea676683dcaf7d6783c3935a63953e47d0ad419f86ccb006f0f02d7b8d168fc3cb5cf5fbfb0fec7181a76d6b1dab94" },
                { "en-CA", "882435e282a28b121fcb4b2d45fc5abfdfa81287f3fb95cead6d9b67b79f56c8cb02d6f4028a4fe03cc1d001dd9e036406c6fff91c2c3c9d86bfa54e24a0b8a8" },
                { "en-GB", "bf45c0ba4b6c798d3901da9fdaba5d1e3ab16f467dbfadc19aa3f5054c3cd28b22998baa5793baa9d18648350d514a65d2f7942514e889fcc581697c7db13f47" },
                { "en-US", "7b5790d4805585efe2bb6e5e04f45e55de8e70f3bc7a8688d74f3a9b4d7eb61c390e239616be56270655e8f2e7a8823543e26f1d0c71380f45870590221f65b9" },
                { "es-AR", "d8a690f31ca93f577645b5b327fc585c76041ce2e4af115427f8bc2944538c9bf584d75f4bf6c014d5297905cb78164d061a72535979d87fbf483b43abccd591" },
                { "es-ES", "fdb6136fc9e1c2d15c854885911321fdd76c3f13b10f5d7f85ff1a4cc90af5dcfe124d00fa10c20fa49e8a5fbe6789d87ed320c56db705a858261c22d067f13c" },
                { "es-MX", "31099e880fcb5271ad81d5505ec84abc39bfe915e8765f10b8b125658201911474bb4ffc35b063c5332b74d7682bcf34fc6aed34c07926abee00e31d5a51f344" },
                { "et", "c06e682c751c2fe5d79c6a45141b5a9a5d772dce463a469a592f4ce4e87c94e184609824c0f14b8fb5dfcd13f3954818d45bc9f20b59c470630c2bee69d927ab" },
                { "eu", "f5ef79fddf5a210de15c6b80b04e9c80beca22c34b90b3ba16ec46f513f236829d0ef6d139b124d9690e695c909d187677a26a8f72eff1f0c5665f4418a3d770" },
                { "fi", "bef9e7ae282d968e9c45a4d87357b594799c6cbf7868a434bb99783d53eaaaabde82b628bdb6c8fd720336c15d96e82ed0814089f962976f9ddf80a1c7436b67" },
                { "fr", "ca65725a6cdc836f61f2e2eee9f792568cbc79d0a5b2f8bb4023c6f50f58d4812b7e0d93212a6c5ecff81d4c58c8ec378e0c9d887ca83bc7e71a23bd9b269145" },
                { "fy-NL", "a5c2762116b4a799efab9337e18a07d16c742c9ac51c3e3ab9d7d279d68776a75d7665fd2ba1f84db637b403a64f2991135875e6bf7659e004cb9fa902b38920" },
                { "ga-IE", "70eb0fd295a937b1dcd586cfd5fc71a03488f2ccd4f905264078c4dea981f7ab7d05d195b6197b8a6b284ffe4fa51b8c2c39518ebae566732f84447c7c93f3e5" },
                { "gd", "947fb9fcde418fbfb86795ce665f763ff8458c198d088e0c1f6c78bf06d23598b9274235fd36aab42f503eb7f6465b36a2b74cea0a243a2c981dd8b98edc5349" },
                { "gl", "7eda15b9e7be394a7518c5077cb7515906239b88f45ba076228f832691c72fe16c1cf53df455725a8e03a304dd94fbaa189ad3395f7e6236003db7e126ba4a7d" },
                { "he", "4e8835ed279a4c083e9ac7a26a72b35c835691276b703da13253d44a477d78b092c2c5a0f46388849ee20fe6910556f508565fa3b3821fa1d76836635a2be19f" },
                { "hr", "19573e4d02d8dd3265cb7ac686a45a92cb14c1c0ac26a71cca6e0ed86e7ca5ca766ffaf3f8d8a9ad4b4fe99d5aa37ecdc62c965d9ce49dcbafa77f62881ab47b" },
                { "hsb", "556c8922530899d5eaf60d1bbdc08e561fa0db96a23d13b5652a13a6909c0e4241550d74417444cd434e923194d5678a03507c8bafe3b252f735090890bb1fbf" },
                { "hu", "500be76b36915af438954c8e3580104b563edff7a6faf635a25d11710ddbbf789ba150f4a41c4976583a77b810491e8b065d294761dededb2f7d9b3acc3e93a4" },
                { "hy-AM", "9da24c82131a88782885dc0ab49914450a8a35e0b4fea2654169bd33a4eba66021f74bea663d7bc8a6f8384b731d7d1c6fceccbf35de9949ef2476b59007aa51" },
                { "id", "d615b35abcc0c92be9a5ab2849e0d44c722f2f87b4f5ea06d3beea7cdd08d036b9fd49cc6d7693ee907473c6e7e730251f99fc1e2074ee396f27636201538922" },
                { "is", "19f54fe6d2e25f68218bfe882b6bbe434cd6c9df8fc540ecf4010e1e84c34a20f562df5160f552f37166f58a7bd836a3ce8a6533648f632c0609a7a0c8ef7742" },
                { "it", "53808e267c668d2c532995727c90752c2b0e67ba21a87fb5883e5efef3827cd6bb1a6040ebd27bb2192b327aa08a5fb8646191861aa1c263a635f79581139270" },
                { "ja", "b69f157fa047f8d7d0173257437fa33a3b8a29374d29d12a68506a96d96dfed7d2630f295973ac5428276672e964097e611241b382fba739175be9795946452b" },
                { "ka", "227553f98eb5eedff7c675cf0de85f7b0ecc04cb5745a829dc1c11758e9b07f4bdbceae16ee4e4fec0ff8fab97d06a91867ea51a42534139de1067b2b1970972" },
                { "kab", "2f37aaed1c34e94987dbf81e36c33d7cb7206631c95af265bc748f46eb99e9f7ab6ca5f6b9454c557505fb93c6e87ce7fed7bef6f4d62eece18b1e3f4fd05b12" },
                { "kk", "48cb08db7fc6b844cf75cedb4d46ad5f52515cc0c36c18f6b329c7d3a31df090ee0a2aa5f2e3042ad1c943f8c830c4c6467a6dde46be4c4dfe624cea613b3e9a" },
                { "ko", "59b7f3f83fce03ec94f05d5585421c545e5937e48cf97a34d0bf3a1caf4b295c14363a0411bd76d0c53a7540bb79e46d19320a8654cb499fdc35bf465fd37bb4" },
                { "lt", "86eac5039f2f6433ce40cb48e4294be3e3176691a5093d334f23d6cfa0c91f509b9462842a43940bea3ba9fd24ff1f12528d0b45657c804b26018cde82eb460f" },
                { "lv", "07b2445851e6e004590d6c88648901904f9000f637554618022bd3361bfe498429717890c3a2c37fb039de33e17f378680a3f4796a411eb95747e57ea4546dc5" },
                { "ms", "3e494f6bab66adf82dc4b99fedad9460999263d7d2abc225363debb03bc0886303290f11dc6bc923efd75cfbffdf6bc8edad3d2f96abc74ff154c16f38a6b1e2" },
                { "nb-NO", "72d4fbaa6918c31fca9fdd201fc28ff6eef95d1b6e25b96becfcf72c8f7eec7f6942e78e59370fbd7e64a202bc3d7d98a7739035ab30658f39489aa2be4efbcd" },
                { "nl", "9fdc73c58ef9486926d89c1bc62348733625ef2f152c96d0a275079a15c35abeb2bdd2f6f59db24a4b009c0613471942c9749e7c8e6025380e1d28baee2f251d" },
                { "nn-NO", "8eb0cb049f62f39fc26928c81508e6cda933bd6012561e79ed51dd76e724598e4427cb83cc7b25deb5fdeb938358129f2d474159218ac5121db98ea17ca3e728" },
                { "pa-IN", "678b2478b532640f5b348abebf84f2e0b9e026b6800f9725df582387af91e70801a5b7327f1ff2d3cc5e8fd87f4a2756e5a91366e1095fcf36e8f02df93d4718" },
                { "pl", "478a6ad1c2149dafcc676f00585fa3917ab208e75dcfc4ef4e00c2c5a6d8483cff03a02b1a0944923cd0db75b82072a2f2e2266c79e28fe2a3cc91130e18ed13" },
                { "pt-BR", "53a206ecd1b3dfc70ffd45415d3294295362ad61601bc9591a041b7cae1685a67b4bd27824af53a0c607a44ec577c4bafe92a9084bef041b643b70b6f9f8ecbf" },
                { "pt-PT", "8b2dc7339ce9dd229fb91c32d2811909a099434a475799772cadbfee601c0b7f6e071197309c56d2c3205061705303af73ff7807b334e6a5ad9791b60896cc01" },
                { "rm", "e2318b72bce98c5ad3b2f31324df6b26c64cb6d943e21ad27c6d532ee7e52c3befdc9cb5d7f0e90ff71858eb676fe19d05ebe1f0443bb1106423542dbd7a2eb5" },
                { "ro", "57c65a5f687770e8b57230ec2965157ac1ef2fb0e20edae08c42b5679470b94e0387fd3f92d08fa7a041b7f391699ebefed8dc6566ccf55f11204872fa94f7ca" },
                { "ru", "146a655d6c45df429ce662c9af0dbea186b57f85a49ce47e09d1eb114a66dcae0726283653f71dc646cc35877e60de01e05035b54dfcb8fd3f07b7cf17beb51f" },
                { "sk", "add661c97b5f8f471422af4768129afc317fe640ff4b564fc3085ea600f2ab94016fc429612525ee79af82f2562331952e490f39f116e3ea640addfde7c87f08" },
                { "sl", "fd5bc02a6c70d6590be74c827135674b7361226b784549bdd71b86b6b5fe326cd329c08dae808a09fcdb8535534f9f283fcf5367239de61abdc9e2a80bf5ec56" },
                { "sq", "c92620e9711a3f5b327b88b720aec11198d7182acecca814487f35160d0fe62b46a67485b2ed9aeeb475d82f462db326acf344b386480f8d1eee0e14105b2501" },
                { "sr", "b69dfb5f763c6d126645bca497f60962a7f0bea6122866678c186a22cf938fb887f60a514751347879b75f9509aa94f908bb4b08ff0206d6b1cf0cfe54986109" },
                { "sv-SE", "1d1b6864b00b7525173aef97bb7be0bb78972a2c4afc58b4d4225d611117fbda1d2e62fa72720725ec51f0767b053b1107e0e4408f1ee4b347eb6693e0938640" },
                { "th", "f29b8da4b13155c9fc4e67db3168859ee074aaad0e8bc9aefa8f26a831f52951edfe2682b0411410001d0617656f9e7549d035040a2daeb7aa996afb8de0d25a" },
                { "tr", "1524c65f81a3c56e228de9157260db7ed02592b0881adae9bfab27c21396cecdcc04d3ca7f4d03c3898db5787f42c24c72cd956649a7d1161773a7e188beaa50" },
                { "uk", "28b768b3261db24f5c4fa6429520a51f2e0e7cff06c51e9f74cb7986875a0d2183feb42bd15ea38d49ac2e69b441c95bd4c0652d5c755478ba6110ae0058c990" },
                { "uz", "f183e508e6d1ec6f9a8df6a17562bc9ed77cd147cac6bbdd913a6b96d706c1c7dae76324d2daa0cb0c4b039c695b3c3bbd9a9da139571fbf47995ba29620ce6a" },
                { "vi", "017a612db92e91a4c0141340fdd7c2d5f45a80c1c27c635d9995c65519cb28a0a64cf8a0768892900cc11065e5cb545930960acdc8f517c9566433ef739a186b" },
                { "zh-CN", "b6b3f2c3b89b2e9fe1a80b1e5f5ec155ae821abfec7297069a9693c47f83fc3d13162fee165ffc5392b0cede3e4bb0e23b2fdacaba738a5c63da02caeab72e16" },
                { "zh-TW", "e51c76d2b3f0924104f4954a9a9765e61070f4c34042840d05a41a06cf6c73252f0c1ca08790e75499ddb070a1bdc8d7f2aeb0bbdc2f4a2b6b09d5d01ba9efdf" }
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
            const string version = "102.11.1";
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
