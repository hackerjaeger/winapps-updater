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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.7.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "d3d3d1311da82a6015ed3e01df98402b741abb05a75f1ef9297ce0f5b0d80ad4d19f027613360ec9b76284e3889f62f25daeb0d4eada3bb8678c3cbce1f6f4aa" },
                { "ar", "deb412e8345545ccaf20dd19d8e9746644f734de4c1872bfb3b907577af8eb100269c9447e4b93e2fcdd34fe69fd239c91123ce1beb1d8e4882cb0e0370738a6" },
                { "ast", "199eccf6bbf3270ab7630ec80a51d2235b37709031996e79358c712cf4a92bd2dd7ddc139472dafb9a4f23a930386867b1ffc4870c5ac56d2b92e5337a0e201c" },
                { "be", "1ced9e5a0a998b05820f522a7b3b3a8a15a0ad7b7f90cd3ce723a1525e61a970f0cdd53a2ff89787e00dfa06503568d20983545d11c2e2b141bf7da45ff4435a" },
                { "bg", "471a8730a33cc3bfa2577123bc2be8f6a8d99a3536eeca9b6662078f9c8885906a59d1a0137231317b6fbf949f9955a8513776560e65e309cee694a250aea1e0" },
                { "br", "6d532abf0f1888889d0f976a1f5066a02c8313f4ca5af27dff4c82eaed3c29b33d4d8df9eae9a02ca7b1a6b2fd76a20f5858e7402e766f0632b15762aaa72f32" },
                { "ca", "818dc5eeb15614e153c5350d9d9716b5183da5ca00dff10c87a17f05159bf2acd4fb8410a7bf154b3ff33efeba467d8cb28c2006cb9276e6df5f3cc18af8433f" },
                { "cak", "26e1ca9d6c04f228750b3e5ebd2f1f9955c552fd8bec791e80366892cd1a6a04e0c70008b80f2015c1c5860de0c4551bd7bb03db4855f4530bd0c4d4a7b236be" },
                { "cs", "20d6062534be983fa9b8c1d54c094aae7f2a036961b3d601f619a56f8bd700eb5be3ac2bc16ba45c0093773b6740f11e08c41b6b71d9cc4cd2274eb07f8c6063" },
                { "cy", "4eeb351afbcf74e2124d51bb6feeb164b2f6fba62d7ef7d0517f28914e32a860e6475664ede43e6dfa4dd1fc436a316b2925086a8c2233e27d87b3cc98d1d65f" },
                { "da", "a536221b1e2d13db7119ee2651c2267faafb5fb4a62049587b6486c1590dfc78d2b2a718798d4bd8ba8da1390e19335069e147b8423f139db7e4e269ef4b8e20" },
                { "de", "961b25cf34dadb56dbbbc6b65978a138f37fc0f51a451a90e80ba424534d1392fdf39b0e78eaa945c8f2f32d1fc7763d3b4a692d16cef8b09cc78d467aed1295" },
                { "dsb", "b513a3b32fb0235a495aa664c3eb660d5e4bff709a3ef25c293b44ef2a15e615d6260a49a932fcb2316b86060bf76561475146877067d79075aad25ef229c57c" },
                { "el", "c86a42d65af2a4275dc204c6e570101f150094466c08b145770c6da7abb0465d550f54245b8e5163941f21010366e69b564be85930c26e4fc345bbe7e59abf43" },
                { "en-CA", "f963378f439df599c782982ab7339365d09b67c1fb8513fe222901092c71d06a7bfb9274c53e5b3d2e3a9e5069524cd5f50f88919b0aadaaa1fca3f6e838ba37" },
                { "en-GB", "a6b2aa6104e9dde033a38f2876147556ed3318653866586b1d942689793b165840fc4437750b5cdcdfa3d571bd88047f50a8a15287de56dbb98154911c4f2589" },
                { "en-US", "00cae3d76c7519b3edf7acdf91b7eb1511067ee89fbb1759b29c23c5d798667f36f1b5650e39b808a19710df7039d33ced62b3bdcb4129840e7933f0f0d8d168" },
                { "es-AR", "8bc1d26d966cf032b0675114f5b7bb5c498d44ec1e9ed4b01f7731b3074849c14c0dd7c879e306c41a9e8f5e77145e7bbb120b7db26f5694dcfaa066d5b2c9b7" },
                { "es-ES", "bf872e5706ce2ff5f97ceb16080983057d6bd9d516641fffe65129b44dff86bf995affc1c4a41b6f646533472e31deaa4a240b5557848482f25718e92b227fea" },
                { "es-MX", "2b88a55bda41b46fbf1403ee3e42e8def72b80e3fa17d147aa5fd24967fa7ccdf27d38ae0779dcf498eb1b0e1a1abac7fce25b972f82282e25e17d7c9f24869f" },
                { "et", "2fd666d98bc4133a8970e97136c198ef41058bfe76d82872102871d7ae160d9b1199ef64eca15de1a103aff867a5d326f3d78e40c9fd876286943663d9049cd3" },
                { "eu", "ce7bac0f1622dd508b2ff5665024ab8211d6d37de9dc023397c74e7a156b40c02d03a4290426b0c62c24adf5e84827745163ff5bd0d60b953a9ceff0423eda87" },
                { "fi", "e81c4e18732910aa6cabcbf0a8ff9489f8ceda24d7a92544df2c4b1a859cb57609de5102863e8d6f72b2db724723bb00ed906fafd2d3a8312d062ff2af7ad52c" },
                { "fr", "db9d998020b99fcb141b2875eeaba93fd211edc4d0c9d907573e96ee640ea522a58de493dda8062c38b55c03fa8ee6c8a3689c7e5b8704c9fddc40945b5b633c" },
                { "fy-NL", "04d1ec4579f652acb253883a279a4dee16791c5618e96373cbddd80ea59f361d945321aebf612335f3e5d524c876edf446888c8dce5896e4549c49723083b7f4" },
                { "ga-IE", "a998a795dce262bf3317d90801edddb626f4e206c55dfd1769ecf4281f03e226285415a1cc1c82942c7710b92701313838c4d6cf1c85f262de3b6d52ef709943" },
                { "gd", "a7a7dd421792a3a6da17a7a137a316789719bf41bc2e438be516d63fad337eca50471922b55282558d136238a6f8a40ec021dbc6084f74b429fed64cc092ab12" },
                { "gl", "1302989e1a3a3ddabeff36707070359e8aeb4f4d3f7c21d4c2ecea5d2efef17ec375ead9a0fd3cad727a327ced2e2c7f70903a29fd7cab7ea5c0bac6d1d9bc8b" },
                { "he", "3ef3f7d818a68238f0055a354c3a46167678255cf95cfa634af3210bcdd35d9157ccb24316afd0d3345b091d95682a8ccdf9d607d30ae29b972f3c45f2931d2a" },
                { "hr", "563e177640f90b3c30fd926e85a6b55bf3748221a723fb5cbc14fd054b44e3c84b53142fa28ba27db59c58c471f743cc3b384df5a3bc75eeedaeca14d70d9bd8" },
                { "hsb", "3dc69cf56d3d02bee69346309b46f6b5e44e5e948489f5fd608525c8c536996d3017d32d63bad9ea6c598036b01c6eae3fedd5216e17c685109d485b29e6ae8f" },
                { "hu", "df3f45b25c8c3da76d2ca6c5db15a56a826ddbb93a797c5ecef4d66b7803ed5355261223b681027682083df20a2389f299c499aabc37a727db43ad2881dac4d2" },
                { "hy-AM", "9ad362358b0daab7dd4a76d20d7643e6f5c993cdbdd882387f601d3df7754bc39c2e8dd62c9bb9ac3bb92f94df3089834f5702304ee184c2dfb990b7c4178a02" },
                { "id", "cc733b75a1cde8d3ae2c5dd6cbb35ee92f86acacf3ce358f9473fd332fb041370fac8c0533a157db02afb034fa93a07d46270a351f117e66bbef614a6f6cb4f5" },
                { "is", "0a0bbcb84298aa2024404048bb48dbd915b9afb013b713d1cd48c51569284d66a68e904bdeaff04368e307ef544be65838c904232f7ff9b6c5a1141d6631caf4" },
                { "it", "6aee96d81eb0f5b28528366c912053aaece0ef17216aea0999b0aad183c83286a3d8fc27b210429575466cee3ba3172d8b6b7bb749852c85feb16b66f2cdd4a5" },
                { "ja", "e04d4b918bdc343bd0b123384a51f0df00c08474c7b99f8290e6f92810fd81e79140c447d8d5758a25c52c2366018383ad204cf20b7e992cfc196ac72352b44c" },
                { "ka", "454a41e3c20eed66a762639ed27176daddce59856ec4579a9769d4ff9482ba32a71921a49a080d541320853ec21c98edaeeccfdf10ccdd4aada16036b4f721cc" },
                { "kab", "06c517b985b76f04cae821f328cac91ed01c2d02f40beeb018c727f65b6e0b3007000263105e62b189ac371570d1654d2dbea2ea6b9e443fa40b0ccd671be97b" },
                { "kk", "a94a19c6afb9c881064a054262dc958df06a8ad45854b71de6287a891bf7388739515abde491c9c378778ddabb3dbee830ded4fdadcfc6c8a551e51e35a9f711" },
                { "ko", "1b4260a1305dcdff714d745e0bb518df77030f375e4d5830918a14a04d3c3ddbae0e65ed59e84ac786ae08c2a19d5c128fa1620e8ab5f9d0ce300afed150c9d7" },
                { "lt", "046a54638274a2fe47252e6b1f621109e843fd757c58031f9fdc47191dcbc2bcb800d0683d88f516d55c96b500e74e7b24348e1a5af208e5992f9c4a3a8abf71" },
                { "lv", "5ac257f9811176b8af8e788c6a6c40a5a5a23f339d9387dee47f7fbaac2dc08e62972c17390ab98adf564c0d1acb6997a637e2eee28e52982b6b45c434e02867" },
                { "ms", "fd44bf0cb1732a20faa0e140bff962189e18e9255838c4df4b1f07ab3fd2065b87943c4576b744530f1190b9beba8c5bbe92cd0a60cc8f348ce9847fe6425435" },
                { "nb-NO", "a3b91ba27a1c009de54afe631080022c623ccb19a3704277142abbd26871532bb0e8f5bc9ae5f38d7e3d609254733ccd50ac23cd429dbbfe693951c6e6ea4bfc" },
                { "nl", "c6e8c494c312e2f849fca9bc00a45b43642ad949538140b820f2e11251347fec25f0db316862ecfc6e62bbb00a57ef11d65d48d381cb752eda6c5427732c6f0e" },
                { "nn-NO", "b1b1ed9ba835532721ed3ddf986c374d1d180091f3d9ef2620d09c03c74e95876896c4bcce9b4acbc74b38f54fe477b9693ee504e6a0301b17ef0b993afaeb2d" },
                { "pa-IN", "8dae602a55999f9683624eb990bb3424bb03a31fdfd8d6219c5b85cf747f04958e95d52e366422fcb8c7c35745ff84654ef35b1cea57a73add0df31e5b7fd86b" },
                { "pl", "485cadb47c9e18741dae7a1004dbfdc68c9538272cc874d23cec3ca1b5f5c7eb18389dc222d44a04ee5139d5557eea9831230dac9969d96626c0dcb638b67175" },
                { "pt-BR", "e681cc56182050ee30444402689e6574db6027a1916cebac63af6e30b3212e7682a3c05acca86bc2de871c6daf615cc3a610f6da8cfd3ead058673096c94cf07" },
                { "pt-PT", "b898340b31cee4d7ca6e6668886a82886f179885b52a391606a25a16fd5d2c737e6168c6d5b21595250d0564c29e305eab7a60b042b94f34badd882097d3a305" },
                { "rm", "a4af485848e465f73e8d985c2a66a636e57ff9b89a5365072f7f2d2133e6906035a927e206e68cb28fbe1c436849f8792b96a951b7c406d97f495de11cbb828e" },
                { "ro", "4745fff61caa2c6430bcb861555862e3e3fadbc9a5d6fb56d0d30369bc9701ca9ba6d7fecc63f508e45379bd7cd6266cfb81bc7b63f106c4737c0aaa3dab1f15" },
                { "ru", "8329aa07b9bad5ccd2688fb27ce71175d69daeb78390f1a706d5ae73373d266599a58d74ee45eaa61ea52ba8ce28170119bef545fce8aca911c54d3fbd8b0ccc" },
                { "sk", "29c10ac0cead815aefa502e1d5b4eeee5414cab8ab71a2adda97e03b0d1f88df5b4bdb6d056e060e393e4da7f41269cf7f3241a6abb983e1d5af62e05062bab8" },
                { "sl", "5d63c79a55e61b73c22d8eda27126785055f6e455c373ad3f4c161c96ba1ed0497163263c80dc06afccfd4274d4e89d8ef2f03195f50e3a9596e580d5b011cc4" },
                { "sq", "726d2835b20518310d897d61bfa487d2f5918e32be9f3b45a66a562db7968b5b30df69e790526c85bc3992fd891df20947786ab75d58f483afc179ca1f1404a3" },
                { "sr", "85fcdbac24f007fc1524f449a3d72d16af1a75ed96406fd538d8ba70b3959a98f81b329ec2c4c32d97052113cfa8ea69437f32536bedf95c7f1a754987fd6818" },
                { "sv-SE", "2dcf779b6c9b692c97b34621f44d8d679b2a6d29eeeb21c0bc95971d5b808fcbe9877cd83c4347608f310ffd6ef56d73bfc971339570f445b53c2e9c4a5dbc4f" },
                { "th", "0955913d5bbc593d0276f3a7470ef87ef9d5a12b327887b157fefa5aeb3b880a9a3cb978077ed9d1ba421789c0af426518bbb51e14c8d5c6cd486c8c779bb1cc" },
                { "tr", "00e08857c9f38c0c8ca80541422d6a327de3456d1270c85a6abfffadb0730b910c7324f487713359fa90ebef2bd8837d7dba8c7125493a5797cf2697322a1c9b" },
                { "uk", "eb6bd6aa163305c8dcbad3dbaca41cc7b774a364ebb31fff60e8b5b447f9356ccb9d4d25d4cc28e3944944d486c03edecabce6e6f695c40ea27c8e0b5d51cb6b" },
                { "uz", "f57daf7d902e3d5562802c053bc697d7b1b82326d638814414a6864689342eacbe12c08d003f9bd2643ffc36a81965b726a4b2f9da7cd62146e387d55bde32e1" },
                { "vi", "45d7d86954b39e5ff559444bd7584993ce4ef117777dda941d151250f4a435882fbb05d19414d010165269454a0f7213511c18203393fd062fd6ecba8c9c7986" },
                { "zh-CN", "d7dd45bc4cdc6c533893b2de326d29dde2b268d32f63c13694fb5ae698471d98da66e1dd2c2566a9f2d3f50d0adee5b148da15475041826ecbf3e812ef3936c5" },
                { "zh-TW", "908a9b9c30d04f6c239c34deaf97251fe767ac7e2db5b90bc0280239eba236753b2ed9682a44d7b5139ebd009b110126fcb66e1a2a0ec05d539fd8585e5a4590" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.7.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "be1aa84c044b962b6ff283014ff7caf1d76243e3624bf99e776641702ce71d107b02a9721842cd1875d14ca35254608fb8b1e8d8dfbb25a916ddbb75207d0453" },
                { "ar", "daf19e29d1d0b36cd3a8525a01282861c4cdfc76aa12938c8b8cbc9c9c07fc469e610668f871d179e19beabf0d1a0aa7c80116677630b89ce3e930d84b248015" },
                { "ast", "1ad78ed49335facf559208440aedff788bec21531df87fba84e7035dfe296ba72bc5ecbb711231686d63fdfc8e46c5146f63083caf4c28a1ae6cfd91c8081b05" },
                { "be", "27476dcc7983fde9de6cd9982b9ff02f977fc64148ebaa5d4b6a143dd0148ed10c3c690139571f8ff0a4588333942b5a6a73631c10120091fbf8f7bf7fbd0901" },
                { "bg", "79063f8a38241f876b4370758f03192939399334a690bb1f4eafa13574bab41dde903d3a61c3eb048d5a72899db70575a39eb946003f8a24566474892904f498" },
                { "br", "d29a99872ded3232a7ec7df48eb78d8b1610b956224d48a73303ef0ddb67834f71912a330f86b41f9ef6cddb95ea604df147fd856f0c0fdc40f6c2093e046ae9" },
                { "ca", "41fec1b0d533ef2e32074871840a609f0f554d1285ce90aaf006f6259ccc5d3685384bdc997ce8e3e16487504720e640befae3071e5ba35a2309512e9f3031e3" },
                { "cak", "8ab04d304aa4e84444873abb0b0e88b76cd11db1c3987ff21947eea751539e1f93531bcc90c1f9da203e5e1125d47aff681987c8c230a7f9d7030b61976399ee" },
                { "cs", "13ef5b03f91bee894ae6323dfed9c670f38e34eb4c80aa5062d2504705527ec323cec8316c185caf2f48c78a7ca44867a70ce2d512cbe79c9a6bb5c786bf52e4" },
                { "cy", "ffdabc97a7ab6ee6a230aacaf8818c35325b42eaa887d449832cec416ee89256cfc97f00c59400d449bac878a51be52669a1fe86b7bf71784c8d49905907cf2e" },
                { "da", "6c5829804c45d9d3afaecd40d71a96ba85b13802927bb332291ba2e4a67d558539f1c2b1b83b2d20b8761f0486f7f579ff9da495d091ac5256552f9e18ffd179" },
                { "de", "c384d4034c4b8c0f5fab2de5a15d3684c298ddb6aeef6a0b727935edc95e29dfc054c3fb6725daa74be1f3de249037a25416037eaa77a916663be64e8c0d6e2b" },
                { "dsb", "02eb717cf0b1b38d30837d3a0f9fe4d61996a41bd82b9d3708b0e74d2cb6f742e8f9034719edc2ae3836e7b0643940da360e068320da3e43c8b6a0b172f104bb" },
                { "el", "6b8d43f0e86e5e40df8658f00e0a32099e42a8ee8e80a538ef4bb5de89072c37ff7e08c90255689e4344dd6c95a26a6306e5ca379138c35e5b043b0e51cc7634" },
                { "en-CA", "f16f5b9df6b8f1f35e75c020beab63ce279e45daa96f604f6b4b3205c892f81665632b3c49a9d0f512e13096e792e42e2134c64120269bf343ad7f3b456325f1" },
                { "en-GB", "4a5d7217acd85ae22d0a0c2103531cb6ca11820698c81d2326c5628a00a770695e76a44f4b0e57346d96834d5a9d058a35f808fb59f034ca30db0abff95ee5de" },
                { "en-US", "85d3e86fed6a4686ddce2192b199ad63ec6c77d54c728e8275a31880e3cb87b2edff83ca9c0e416c9e1fc8fdae9ffa15e393aa0f06a3b96c960c5d1fb87c0e74" },
                { "es-AR", "c50681e5b2b823ecc15948af117b59ba9a169778b9a2881c050c85163e6b620cb0053bbbe3dcc3c6bed2016a6fcbb0eb76e033e0830b5b242f17a2658c60d9c1" },
                { "es-ES", "9f0051e171bb6f05cf47deb75652c9b7c50e0b7b82793bf135ac0ef6ebae3f8cb376bb099f21666c2f4ee20cdcee75717f7f5087700f4c1cb346611f9567f91e" },
                { "es-MX", "f0bffea053af17651f3ba69dbc828cc2a21f58681065ad9cb78c3c76df162bbb8ed36d1e7983e5c6bae8c9fc7948ba8d200405c6274fa87496d1c1e4dfbc574d" },
                { "et", "41287a4a997c67d658aedf335c2fc30da7d1eb98a7f2adfdea1307032d8fd7a75a67f827e6ca897a986ecbb2c4743911ee423451f8164a32666913de757a1b95" },
                { "eu", "99c776c8f54b22ece07be67cfed9c9b26bb4b50b33a46085a45bc3d1cef1bfeed058b05152765555a0a7b9bcfd1eaaefd8a3609c3f1b276699760ccb6ded28b3" },
                { "fi", "4a8a9ed9b25d596fc2f1743998956cd7caa7050a6da6e2c854e77131ee56c1fb5393d791d8b1686fa2f1276dc72dd97d5839dd1a7a64092d88e8450dd610e615" },
                { "fr", "306b78efd864d02aa67a943b839204ea5e27c61794d401582f9ec9a2ada1e679f3b1626f12006fa0c57f23881476dc2418b2d3406efcabcb4a3aeb328551bd46" },
                { "fy-NL", "03b6a293bd51bc3ba1f9ca5f43762c9a64c2aa9803da87153101326c5dbfcf58dc94bf7b0774bc10a9849ca8557cdf13382d15895345d7f3ee48474f9339dc36" },
                { "ga-IE", "f897a8c82edc2345719981a7718cb7ed32e2592fab9da087e69146f935471a392757613a72416829f7f87381a43ce7e3d17a3dc357eb32d4fd88a5a44c331000" },
                { "gd", "c534338d0b7fbc122b4c591a1bda32fc72c115ec77c983a24c482b14c6ebff282fc6403aa8c5162faa384676e18cb4d8c43e260b9bea5abf3402f4c122e49159" },
                { "gl", "5e0bbaca6d417869b5ad029027989f4b8d9b1f99c8a0f8804c5b1607d50506bd105212fcf960d342741249fe81cb55a497c03af96e2e2f9a58f7640044ea774e" },
                { "he", "266d7ce4477d2852031bb62904dcc6617883b2af7d6e1ec865832755150a13fee3431ebb784f8e157fecb3c34adcc65dc21c50eb6399a8b0eff52fe6342a493d" },
                { "hr", "aef60c67069adbba5da36800505c2a7e81fe42bfd1f9cfe53d79a71c7c32c2e4d7da1c5f213182c9d9b99fa54f88ea95510734e60bb799b53b6079aeea5ee6be" },
                { "hsb", "d1f06ce382212c966162af97a69fb9c063cf2720875b8050b687fa450603298500dd39f08d1bd7af7d9897834262f021a4c637dc5ff26f3d2723f2f6570ba001" },
                { "hu", "40b060d5106f72dbe4d03bc13528294257c8d305215d01077d2307f0f6525ccc3cccaaa6f100de83bb9146ffd19f799db2b8149e1dc58056b0477f40bf3178e2" },
                { "hy-AM", "e975aad14288c089c3d5f3dc5571b0923c5706e4c5b47196b9616f22c5fdf56372625315cea529e2ae567734f263998863a786ce1ed7861c392990653c3c5e19" },
                { "id", "a26f71fc033f1875eb88a976f7e19baf62283136fee821468bde47fe96825e55a3198db9493ac9f8aab0e1566c713970c8a309ac0d1e38bfde8677f59edcf0ba" },
                { "is", "e08a33bace74fa525112073cf0451480703ce6abd06fb66b08cb9ba2c7f680c06b17d879d9065be4156167af781e94aa00a2586d7aa6acbcf823033a9432c4e7" },
                { "it", "d953d6de30a2a2e83e89a75fab579d62e25766331a24a1abf189473de56791ce1f664d598f5c64b8e2a06d28e2cb62fef784bfccd9f05728c759eb6e76a5de7c" },
                { "ja", "d2aaeb500d696ee33952c4709065937b700836d4a8f6fe2dfab7de153c54f39a0d20fc7f0443dda16a6a53daec3e9fde203b91398ea4cb1e78952bf17da8121e" },
                { "ka", "85b9d0a69caf99b6c2a82ccc2b9015cf7c878c6ea38fc06e27b1ab06a23e6663c43e74e41ddef7e2a634c978282e0d91d2bf82e15644c430f439d0cb92e5cabc" },
                { "kab", "b2e90f28e93f5d6e52fcf9a9752be42b7fdc0e97c239a4d903323cdcded3c27d181f1bdc891f861fe7f8bd33cc6e2bb3d41efcebb8a6a75d1d79cdab0ba3dce0" },
                { "kk", "01ee6cbd173330efe259219a784bec7b7a2e2fb8f9dee7bab42d02bb85efd3d385128633bc48efc5482b1aad51530f4402096e79834fd4ee90228efb205c8703" },
                { "ko", "4ee7c4b30d5840cdd8660645794420f05bef0cfcc71c6be88cb4e3ece2dffd3d2c60ca49457339439734044f9f04485fc9c0d8e223a80f66b3c09c20e8edc137" },
                { "lt", "a7d196bfb12f919372016a73f60a528fa456fecb0fae48ea8333a31c76824cf378b375a00ddca77c94f988168b12967a45236992455274c5f5b2b0648ccdbb10" },
                { "lv", "ce8600d1672fc74ac87e7ddfaead4bc550c0537f73781b2e54264df2e70f5578e2193ed69b065181f2bbe1ff3c769cfbeb3673fa500144babfe0526d00a6e3a8" },
                { "ms", "10cc9ff4173635680231807dbc8dcfb2550a6ca49d8a6925b9844cad227e3fa692503c4aa12c5a17c2f1873bbd415b7c1e43f695ed41dd29c200dcc7639859ae" },
                { "nb-NO", "e78ff6bab93769193ec8766383d39a28700fdc0a9915ad7d06923089b048797c6e44af95b246326207a740f901b84a170290768d4f5c59dab25c28a5067ffffd" },
                { "nl", "9d79f7be3b8c076a035334e8b74930c1226db2a1803aaaa6b30d18db3efdbdc5ed9a2175862d2fa8454f381251a7e65bdd7aee66d58492c070fa78ec2f45ef2d" },
                { "nn-NO", "df002dd7421c1f4d1cc908b334329a733571f9cacef779ce586ad894f25076154525f954ad5c9283adf5fd55fb6c4b1bd750e3c1ff87c22fea0a041cffcb8cee" },
                { "pa-IN", "fb0f40d0d3ab71a2aed0d148d433452abfa2e12c5a5f830985cf80dbb3cbbe89616d78a03191be11befb449ad3d982ffce7ea7ae262ec190ff878891661f5869" },
                { "pl", "8c635ec14521cee404afbd6f6741b5672a8bd758afc7a346d066613417734ef785e0fc96bf6686699a1b155ffd9b3dc83df728627b04c9f232bf77658a319c29" },
                { "pt-BR", "72b93671636e125d7fd43cf52b79c060da606507b390fb60864fb62e191071b6a864b3045ae11a1b2281dd29dfe851bd9812ee3aa602c2b8a4fbd9a19cb0a4c2" },
                { "pt-PT", "6e1b6af2071dbd2aac4d4f8d437310da4aa4cc6964dc56bf7b791cdbb38d8c42a9b2f7b53e42192ba1ae3d86e10ea4620bcd82d19db17bf5ce9dc544bb3af211" },
                { "rm", "c66ed73ecd30e13be6f34b8df477a33141a55fc102917f99f6163a97ea0863f51b5649f9d47b7ab48be22f7b75f04808473b2e92818a265a15d4ad4464a81c3b" },
                { "ro", "a5db4631ff402142bc3b29120d63cc1a9b0930a8bc5d91f4b3c32a9816d0f0c0875ee4daaa4e804d4afe22720207179ab79dac6fb63570cfbc6befde4d57cc10" },
                { "ru", "4fb932f075e29288707286cf448d3a8ad8f6b386af01d19ff2e791d5e6f8a58064fe9bfe301e658e6efcf829f07d1c16736063505bc691adf019ec8fef0387b0" },
                { "sk", "d41a089e97200ae2c4a329bb0ad2d0d78a163c24f320f98c7520d1fbd23abbb1fb78230e1a28752a6a13ec02cf545591848cc6659f9e4b60ae0239ee2d6a186a" },
                { "sl", "29c7c58dbd28d0060003f6769a5f3f2f76700b09fdcc218a87eef7a0ef36c662bc4e002e6e08a11e306b463c3c514d36018fa03d31907ba178319a9743a574a9" },
                { "sq", "a967d767e3b8e279175bf9db63d84864cd903687ff1aa238ca465f5d591cc9002a014308d0f5636efc2af339f89deae82bb8a0a6d3742ec8e5a53f3484d87e5f" },
                { "sr", "be3649f5d02891a4b99ae2889ba246dad31890d0223f1861aa738ecb29f5a22eeb9c5e5dac0e09d19e7274c3c6124ef8196a75f9745da69364448f8d4d60b431" },
                { "sv-SE", "40da7b79786959fc1362e922940faafc08893ae1e8f1669ddd1aceb95935c1798b51d536ad44b2331387a8089dde689c5bfb7c9092fa8d189bbaa85c603ac3a5" },
                { "th", "dde62dff7e9a8db2a6f0af0eb0f0314b86fb3f1c30fd26cb53f5f2155bf0ac402126123a43861bde08b8efa62042bf6960ce6feb03eae0e5d62c2fad89c6394f" },
                { "tr", "3683d14ffe8c5b11ff0bced6cba453fc37b4f3424c188b67aaae4cf774560ee5d4ef3059fbd224dd7e181eb58a0d5fac844a474df7416c892d638cf0dc5c1ee4" },
                { "uk", "d1f9858b91984f804d9d77ac87a7ef57db2daadf5b3f72dcc894615eaf6df58b201f5dcf9606a692277d84a28a87303c1bec047b7cc74f95f03c7c54ee869763" },
                { "uz", "28941d891fee3bf05906a76e7a98d803f3a445f8a48f9536b59a9b24a3cc4e3c7c7d62f67aff0481f2b008eab6f18614b9b1e362c1684b0916d16c1473d81b60" },
                { "vi", "e18cf4d65b4e4eb73d0682c96a466859315004b35b07abb4471a5c9b4413199ff1269460facaecea587b7e938f0ae792b770454b47f2e845d44cf302c55c4184" },
                { "zh-CN", "ba574d9098624e6ccc49fd27cf42daf5dc4be136e854dbb1509f6490eb385239604d23a8d1bf6d36b1f68806e73ea09a80e09793cac822499a9822ce2316b664" },
                { "zh-TW", "8a3fb614ed72e819a72c9b7654c686651b90b182b26bf48c2e5e4665512ab9fc767a4845126be0ebeac5078d6e0c0b2fc9780ba5849287b5b9e94e37ccfbf188" }
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
            const string version = "102.7.0";
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
