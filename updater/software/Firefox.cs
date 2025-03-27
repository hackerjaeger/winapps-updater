﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "1eddea3a229c4f9f49df98b3215679b14f6f56e3a274bc7c26be6b4799b927fecc5f96e595ae08763f716cd6add373c61c3d57db0d789fff809c64907e1bb59e" },
                { "af", "dd9aef9fcb9a4aeafe63ab2c0bbed9ed5500ab6cfe75b2d56d36bf9b94d7c25d35e85c9cb02cd92c0cc05f6016d18307a609d0b3255b06b2021d7816d5ff6910" },
                { "an", "f62bd0e5cfd593adba85ecf2b7a7a3402d20be588c7bcc64e6e3174a85e7d473163ae24b05c0ec580071b98d2ac0b765f4daaf3ea3f311e2b8bb9c13bb0ef109" },
                { "ar", "e46de7face195792d4339f7d2593ff912bdb865d9ea3632447b3787b4e186b3b644c42171af75fdb3c1054742919f50b8a3a8d34a4d35b641beceb353a6c8f0b" },
                { "ast", "b3c177b7f6cfe9306dc58b9a6b03ca966c0aab8823cc65b08e950d2c5f94ea6dbae43aca3f9ecce842509983d7b7ecff2663b1d20a10f5993f9147602c80ec1e" },
                { "az", "c063a53c0bc8f930078f48881d8de3f2f0326dbb5f653e527b76c8a7c4b7c8c159f0741fae1da9b39c0e7ab81244490bf14bdb17bb007adf480c6b4cc95efb6d" },
                { "be", "e666e9f65c7e0b3a428e2edab916c66e84672ce7e9cf5fdbbd475f18182965dba8e5d084313d5e840ac774c0d71a245946611a697211047c695edc0ed4df0ff0" },
                { "bg", "1053e1bcf1071ece31c760e01507d21761a767b37b770fcf0528948ddb1159fa1c8eb6d52db7be20b2ed25205998bc4bd16afb1baea69b9a589f82a6b46a8bbb" },
                { "bn", "e9c99985ce1bf6d59cfb1a866ad7bf18cdc1f7af4a13f9a3de7ec898c877b6cab5a15aaccc426b68c03bbe9c468be20b1c3f04da4ff49f8e8bc9b79ff668d5bd" },
                { "br", "6a30d997f0b44ee2304431ef06ee3a7983ab06c96b4c34b4c9c67d47410df3334eedfa5aa380314dabfcd2859293fc72e2f75177cf7ea5be837b557921718a1a" },
                { "bs", "66413a24b3195fb9e7543e9180102dbb85fd207fc0d1ddeb86807e005c30a32923fba1d92fb49b99e09c358726f4811761b7ba598a9e868ca5fac6a4361c01c7" },
                { "ca", "23a10ca2c3b37829794a0e382c9f79fb03b8b8a926de436b7aeae1200984cf5c67ded3c244949b402076dbcfd103ae108e5bfc4d5b9a0181a878495643b48df9" },
                { "cak", "93a39773e21b8c4981eb4ce47869f768a780b29d7d8d11984674a71577a8ec7b95528b417a22924c1ad0dfb05af6aab638d6e503183be88c3086aa49b764bf28" },
                { "cs", "aadcbdceae1d9aee54a0648418aad0e8c4011440728a80411f18f097b53cd9b01c063e6883d06988ae269bf35f0caea66ac8ecc268bc1a0e80f804d4024a7b0c" },
                { "cy", "4f2f07132728adca8ef3b09ad73631e7f1d78c4f56cb172f39589891656a13ebbbe1b26ceac909493946106f28f1fed36e75d92164c3371b203a135f45eeb4de" },
                { "da", "a62bbd3da5ba89076f43ef4947dbe452d18a678c25b68517a7e0deb0fd5dc01b4362b072749e6feb32d5829ded55ae87eaaecb554390946b00033365bb838acd" },
                { "de", "c0f23dafc97d52dc702a2af01c03603e7a33c3c842d6531d5f509cd47e68aefa7cbc8031e04f5bdfb6513032dbaefbc32dff0607d89bdc92f00c538b56974da5" },
                { "dsb", "e9cbe39cdc2d23cd7765cb201939a981ecec88702a133697113fb1ab72351f8f10eab59993f4cba908b5cc251e1858a0a74635af18bc3505e81560a94eb845d5" },
                { "el", "b927e1fb244195c3606cfd7e1e677c64d81245ed098be5c8411770108fe97d63611cc2b73acccfe619d62b588a93fa12b39439d3e6f84021e846a86ff0f5c747" },
                { "en-CA", "eb6b4441c42eba6d897f84fcad536e6e7c4a1f0cd4d76b243346fe5bfa5465186d44fd776c7bb0e66d19d856935bf382927d4dd488ce42300d1432ad1cef7ee6" },
                { "en-GB", "07e5195a272aee0782e619fd02b20b012741b91454d238990d71501757bba534b5acd48241d04a06481513148476dea1f033b6b6cf27f9cb4e6d95e45049ffe4" },
                { "en-US", "bab00906daee07ac863ced9c0b39a74efd863731513efe5aaee8db7ae9a8f6b31dd3fff527f63fabf624b0dee8ffaee6a9cc05d87247f2da9e728c9654c77a1b" },
                { "eo", "8f4d31dad39dd70677b8b3500919cc53ba2c0c613195cdac31735045b67482147c82872741f926c2c6b8a600d51bc06d9219f43d62cfa2ab5d2186ef319780b1" },
                { "es-AR", "6825ec2522f112c09f60800bfd1169715b704f4b1e8c9d3217610e2daf44d3eba4dcb3c3b3ae2ce0aab2b8bbd4bb372a3830f23c6fbf1b378c78537a6617a2af" },
                { "es-CL", "2f99ef5598cace91b0876bed167fb65b82cd31a96a6465dc7f94791e9eb67039329aa820c9f024932d912f18fe19c205a93d21e85ccfc4a9cc5a28194b033cb6" },
                { "es-ES", "253c0bb524cf65e22b8b03c0ca78f8c59e9155223d4a519e08adce4718329ff6eee94eac04cab5ae531208cd0b671088023eda5160fa2588e1b3fa5bfd2a99a4" },
                { "es-MX", "ac6d4a95f8a8d6921cc349bee196b62cd94d515d2904cebcd50a477e6c575a7e257a3aee6a1fd8b4ce0221a45a3d399a7aadf6cbd0581c3511ffa257f173c4e8" },
                { "et", "303f42d078ba263ec513fe3f49d4242c4f9371b4ff3dfa763723bcd37e828f7333b9efc1d38637ac553ffb2a1f70752491f0732ceb8e0b2aca3bcbdb8b452142" },
                { "eu", "2d25c384ad244bcc1581961f09aae6163494bd6823478eac2e9b52f052ab6b40ea209eafbfa7a6f0e996a68b894a8092cc11fcb7c2ddc175341fa12aada81d10" },
                { "fa", "0f11d3fd7b130065d50830f76fb6b63999f6814e913693856cc1717c423cb1b6298ab82939a72a91c792f4d57220232b9fb1edd38b695f73807993580e23b59c" },
                { "ff", "261b2b324575eacde5c071f58c40eeb9df5bd9c94dcfa4172d458b7fee47e4eb21be2510999d3104d49b049d2a67e8509584a13451035bc655c598e2f7563909" },
                { "fi", "98a0b673a838b2d7bef53ffb8dd912af303149e260cd1cad636554bb16ea3fe0b0fd1e79beac2ec49cd3b535a659bd22cd66f30671c31ac769abd3227f813ccb" },
                { "fr", "e5b040b1df528b0d238fcb595a90101c7709b28917c8506b4e6bf0267cda4923574a4d9df5e20bf231199c554c5651e21ac11a805c9c907c8eb7ea280fdb7265" },
                { "fur", "75674331cdac46dafd8792c92e6e180360cf0b7661972168a963ff13f454a6103a6a36a367bc6e26f1ff8580814d8ea1217c415084095334991f4eb1119e41db" },
                { "fy-NL", "fcdc467fe387b9d786527e829985134428f4b7a4c13550f60065debd8bb11095d3f4c89d45a36ceda060b8bc1cd00a5caa20809b529698b360b43e85bae6dba9" },
                { "ga-IE", "b51dd19fee533e0201744fc6250220ede3d5aa318b0bb1517af0ec1f79d89a7f9f6d12baf777189a6b9da8f0faadeeacf799acd204fac563ee5438ab6d2bc085" },
                { "gd", "422c4228703cba1fc35c0b482f91b93cb724f8576d2255c6db51adc044c41cd8d1dd2f457dbe19074770c54c7a8ee3d7ef1353caca63a807cbf6d4fc84366a50" },
                { "gl", "31b6b285ff11bf244645f09ea5bde3f41f8a556f41deb56207e3aead04a21390f936e8238d262398a0db0b1bde863bcb03de1973646fed86fee742ea1833670c" },
                { "gn", "e8ad44f3ab6351cb88656927c49f6ecaf4a947ffc0ef95406ba2f5f07b31d1f4b3ecdf6667109c08ad8e1cedd877cd1759688fd7d8ae0933786e9b475f642ef7" },
                { "gu-IN", "d17388e08822f9968ddb5df1a38a1407bce44de92abf014d63576a46f7018d3df9643e20462f5a4d0a848d307e8b60b4e2afc777b395a9a4c8a0b42cc897bd4d" },
                { "he", "7e2c3df66fd4aa72cbb3eb5428881478ddad1bd441f973466f913495cf9ff13cc5be862e36bd8150c7f635f05345d1d94bfee7c9e8785d422f1bd3b722fb3c8f" },
                { "hi-IN", "39c487c9446c6401ac8f738cee023ca4e9b79bc6191ba6791719c5c89676711b453cae096148c53826890acbdfddc2b99db424a75ad570c4f310dbe72f7a8595" },
                { "hr", "ac3649b35252405a2979c1bbe1e65e1f4fb434e9af5712b52e3c02563128514d6e476ea695b86708535312c2a8f9c5475c39ca502d47ca307a36c87f57bec4aa" },
                { "hsb", "9b423f45f90cc9c448ccd736f99362df69256ca86bc217043224291ba394ac825e435c02c82bb3dc1b90e00175c4299aa30136363db86ae6f62b1a82c78f8fca" },
                { "hu", "aa3618ee2932e80cc9d4feaff768ec3ba9e0b65118bf36f3c9b33dfe6a79358b749dfe462571d3642964db97fbe3f973746cf4aeb2c1fe297344def39ee897ea" },
                { "hy-AM", "9f55c9ffc817095311cd9e41afe4436fecbc7640e5d2e75619e3e1553237d7f4da64a8ba627d1dba4901d9ebc359fa0b9f6ce45404e7ecc6eeaaabb63fb16d20" },
                { "ia", "2fef353973397dee1e578e6d3eb9527cb3e67ed3006187ef9f35f555e14bfe2aa122311fbb984bb9158c37e21a725d16447298f48e9e2a2b45a592b58485e21b" },
                { "id", "36b2346859795fcbec2e17d68eab8f9b6cf244820d40b90775dfad96c09a5cb903c72a8320fbd7c419b506fb0da0e9771d62e885450c6bc4cf34d5283621fb6a" },
                { "is", "6c5ca268776ad425673642570ded204436e114cfb546ff6ce403785fa0aa073ca4e49628206210e14db7cd8b9186e7c1188a174227d51d3187990a9857025cfd" },
                { "it", "f43d9214a39c3609e04508662e785a62ef9b9cbd7d5f2ec355138809f3d4fbe4c9b0688e962c70b6d06aa47c8e39aa7890c52db619e0679e2d397d85242d60d6" },
                { "ja", "5f798f78dfe26410af972a1e0413e61af19c46731128f53e59e067817120b8803e6b097d318fe59cd9ed64fb242bce48da141a32b3a3d4d80c3de45de58ec2be" },
                { "ka", "aa5b43560c45a6336dad63a327760e4560938b993fcf160e0036a32a7503acbecbbe209cc392c5f4ce38bed089fa85a50a71bed64a20fd25ea58dbfe846a39a5" },
                { "kab", "e36cad18edd3a97397eabe562598079ec91edbdbf61706aa5d7013aafa6564aa67f4c6351de732ed124fcef4aa86f53ab2919562f320dfd2b7506bc5380229b7" },
                { "kk", "c46e1f2cec3be356a1a42e0c80efe92ff278a905b8e7d7aa77461c8bdf60fb838d246c0d1513dd99a20577c6f76bcc99f016fb6f4f6c4c37875025dec5b7518f" },
                { "km", "1b867ecea85f9720c3476bfa8b965dfb3bcd1ea7cc2b2bf0e72c8fbd4f8c3e531183a2d4fea0bf3367cf7b7fd96f9549db7f3001c83c229880e323fdf2cad563" },
                { "kn", "f4cf592e690666ec87215d006447b2e1f17d83436a72739abde516d5fa957580b313a681e3d594d77ef944f9849e3151bff801df46c5123b08082d8d7628fd63" },
                { "ko", "70f5abc0664069c4b04ea0a68aed795ef3dcf052f4974f2caccdefe4fd9c137caeefa3960034bd88954a948a5ba3168fe10c6f1009686f5231eca38d47f846a8" },
                { "lij", "1d86eab959c190d758ec2f9ba04b79f1b91b4167fb4cd96f48f2bcb7873346da59cba1fd6953c9cc7f88b3e8f0614bb2dcaf2b33dd4374d43e75ba5c4c82abbb" },
                { "lt", "1aff2cbe4f04d10af39afe0244093f6565a00a87c913947863d092477881b5a71748332ecb19da22728281199a952c5a548967dfa737566cfc3e1033f33fd647" },
                { "lv", "86b1adb250e1c4dd611048dd82ea042ff91a59b6a4e8b2e06450f4d8557147dcee4f098550ab44baa8bc91a41955e66327d2cfe4c2da6bd53f0a0a3334a18e43" },
                { "mk", "59b83ecbd9dc03bda52f33235403486b262deb509d05a9e784c60fcdc13aebf55820b24f9339715997b6837cb27bf7a91fe69892bba63462a5dc7bcf268a1e95" },
                { "mr", "6d4869171d600956293cd116e7615cf026b0d40c90252960aed537a785147292a2029dc9e5776bd5401c672edd947752f948e6454a7fd4c426d7183176c2d13c" },
                { "ms", "91e83993d100075f017214a48000383bd6e36ba06adae68aa765d2272a7b53b35093ea32c9bf664fcdb6096b8da8ec41029b44cc408b3bed25e0c92529efecea" },
                { "my", "9433dc1865259260ae4342b19a527aa22ad9e194b3fa7796f92eb7133d4b9158d77b78eee94e34c7deb1d63700b1cedc1b89f953d448b6ed4d21810e94656b59" },
                { "nb-NO", "de0b07dfe19af3c956620a3398f2164dcba6d60f252fa8dd4cc53b7c5e1496f1af3dcacc241aa5a37083d7059dd9ced7bb3e9b19ba64dc3aa1214dcd18ce9d91" },
                { "ne-NP", "f2f8a883b4389148b2fe681cd839499e3c46abe6659c0e67385fb340d6a466aaa2a19cdf9ddc8e9ccff5dfb6c7e0a7e67b13cea44333f58c53bff5a0043276d8" },
                { "nl", "1af98bed057d64d145a1458279592ce5387eefb5e2398b8ee4ed3f3f08d17a7d17fd40e61ba0ec170fe2c9a9c9ca60a641b2846035f8591ed1cfb27739f705fd" },
                { "nn-NO", "eedc000b72c205b8809bac7dca7d9da9bdf957fcff97f8fd3c23f5904add48f262d0251fe1c8ed843eaf7e381058f88117b837c90a04303a2272870ff11cf1f6" },
                { "oc", "22fe2eeebe1734e71a8295466db87edd688048f7c6a1a261921086afeb47d7e1968c09a391ca3da115fde9340d52c09ad3f9a6d02864bcac0f38b47d36b36068" },
                { "pa-IN", "0f2b8cecec4a0dd4ac47de02c609b35182212907f5c380d782d88e2e73a52ed05c0eeb88cc912d0c16a983879c55421d4f274ce04efeb5315c22aaafcb4965c0" },
                { "pl", "757539c4a645f55354bf265c2467691ccbd1c9e16a942647dd58a9bcc95a1fc05f43016e0c67f7b7b80db9085d9d8136ae6d317e53c5506936982450c2795ee3" },
                { "pt-BR", "33c54a0ae903b93a8912d589c469fe74c10e0ef836c40021edd1d207040ed0b9ee332d170d2e18ecaf65fd51ef7d5d4d1103b9164d019821379b8ad85f19193e" },
                { "pt-PT", "dceeec332732ec6db6ec371e9b21abaaa1e613fe39c2e28ad19dc32b3fe1150d7f7ee620c9f0f0af1106dcb3c9a60ba3b2513f4463e1f0e95af9af55c08fd46b" },
                { "rm", "1f08ed7480357f05bc78caf746e238d4dca74019fb89ed457080cca053566eee78ccf1c47bfb210aa57e9228c3110b4cbd645c1d463e163fd36a1eb7f9ebe917" },
                { "ro", "531318b8cace056d408713a567735abc0932cd3e7350ec638f4025c81ab9ccc444ee9d5cc237ea800befd659e6351b797f2db7dd70eda2605ff9367cc16c0674" },
                { "ru", "009e02b661c2555cf61095402f7a1f97da3aabeb933b5294ef9089f6efae591df812e8560b94f3ab7b32dddb8110fb55f96ba77f14b2bef943732e017b26a058" },
                { "sat", "b582a8bdb8092b5fc366854f43cc9e9de2ab06f0275cdda9e57f5ab1d4038aa5039d97bf6b24361ade0ff6391b64dd82fcd1d12fe838a749876197618c249c56" },
                { "sc", "023d6e7ceed7475fe7edeba2686735214c7e36106c2889809ce9bc6741b0f2ee891f343f65910462945dcd8c41c392341c0d6c32da3b3cb7631eb3c70928ac1a" },
                { "sco", "f904220c4be92e7e53674322ebf72760b48cac86064e73024c9ad3cf32acfa1f705e63413c4c591435f1d8a74afeb7ce98678252512ae54df67c09303999469d" },
                { "si", "412f80f89e71209a4ff4a5203fa078ac202b26187bdb71c973a49af0e64052d2ddf612877438247f8b44ab81d27cc11d99800eced5c66b1c5ba28cb30d53b6a9" },
                { "sk", "8919b6b797d21de4ab7ff26c68597d0b1001c3681ff8165134d0483dda34bc8ce8f76d3158f246854ae3d321553f103f25266dc42b30ace4a1195d8e51138d53" },
                { "skr", "0a48c202e9f9aff2fb5003f0c869d6b6967caf9b84b136e7245b4c67586658fddb5225da7b7ac3e83011c2dbfbf2455725dfc5a03def7169e00436c83475da26" },
                { "sl", "e94b6fe79a49908ac8860ffc1a160e3f8638cbec29610fd05328bca5888257a51ec05d37d5886e22f7958b6f67dd49d05b837e8d43a9f8ec48e8495bb5c9cad3" },
                { "son", "865777e2d743233816ea6a1355a9823976bb2bc196bf46f9fe6fa6e27aeb0d4ff2c925e01540d379fad897b6fa39e653db3a5b1183eeb85db6b03c72babf934e" },
                { "sq", "debc3a6f7bd9c1ed37ea51d84689310aeefd9576ff13a06f78940301f88fb9c16edcd0cff2faa3a38bb2c864d1002c44ad393e80f2d18d36982970d932070519" },
                { "sr", "af19a0559b4922cff4b813f59f7ac89a1decd8fda636cb849375e9ec2beee1fd99a231960870327b12bb4b4288a854ac28d54d36ed279959285c2e8be322f22d" },
                { "sv-SE", "bc1389805155552e90ca5b50c537aa634cdc5f5cac8bf0ba58e9514283661d82daf04d91c88f6476284556073d8c330debb89570e254f050405c5bd7b6bae296" },
                { "szl", "e376609c6a03039fdc240fb9b2e6bfdab90e4822fd0bc59e90fe6f2bc7c5af4a37a41413d01b2a164c257b312b3d63b4d44eb35f881eeddc53e9b50160491410" },
                { "ta", "da68dd0b851e0c32c0f86b9ccacc65ec50bebc910f3faec236a51b767b700588a21e47a0f0e60a4b2faf74240c08103512ca88b62b83add3d30f9404c4654320" },
                { "te", "e521b583097aaf1ce96b0e4f39b8d3f95622bfd4a47f8cdd237b2b07ef59d03f251e81b0e8546d52b701b84256bffab71e0ada2e3d6a5c2b018eb60442271d5d" },
                { "tg", "bdf0f73298f2098868dcbdb86f30aa0c3c606ccd2dbdb9138dd66245a58f99936f5853c5dcac9de43ca32da8b75a5d90971575f58a91ec8a4a4a6f392c8bfc4c" },
                { "th", "45b4d93074aeab932b6fa23c58ae60bc8f8f584f87ee184339a4bf39a3a9b7b968441d2c39ad9d4afe62c31c97054ba0a579788d4c93a8a3a46a8c4f04d515bc" },
                { "tl", "17c435f8aa9ecd34c369768dcf3f03b356914b62b0964c50bdb81b959a93ba499838bdd6aa7e3da700237d37a63667a9d5e0e955c58ddb192e9633c0b1eb57e2" },
                { "tr", "0fbfd3a36c97318fbc8cc3a67b69ff5de6c0da089960209c5d4ad3fc3c4f707cbd15e30034f23f68932332ed1f20fe257bad4f4e05233d32f46d3423f74499ed" },
                { "trs", "dfcdf6dae275166a119861cc997c8acbc45742ae6c4f2086310be1683f81361195881a7d614f3ad3fcf91129822f3d93b9894a6f5a5d8ff7185a5fb099ee2752" },
                { "uk", "3222c9cdf9ab36237b94babce245ff45e85e9431492d48c5001be5285647d348badd102c979e76622c068eac79fa28bbcd76978c9581bfaeb63c7185be1cfc77" },
                { "ur", "1a8aa9ac57f7995e45276d828d42a2eab74681ae7e1bff4792a195de711cb9e2401a8ed3a1c7569045aaae49c1fcc6fe4974a8009842fdfb54ef34eb54ef677b" },
                { "uz", "638e692475170c51d442a6058cabc0ef113df8f9970267f7146a4784d5229907c252aae766a46f9c03c1c5ce667604b94f8ff2864de3318026c78db1b345becf" },
                { "vi", "b85f34addf75fac5e9d81c184c486c00b9018320fc676a5f180762b2eec4a6d18247549797efcdc61a77cfec664a956afd612fb266c854e88c381e1e28bd6f0e" },
                { "xh", "68eda5e1055c000dbfc44bd283ea8722b14e3854c0d51047e4897f1bffdfdb6c7af77678b0307f86442d752cc07c970384a4b0baf344c703d1dcb1a493e8c6da" },
                { "zh-CN", "f220689a39159c060d887e6e11f8855317631f9eca91938a8978bc3ac968eedd96f56d297e80fac3fdedd9cb662c34c1e8b949e8115d8b8d7d4f27048c0045f6" },
                { "zh-TW", "2488b3873fdbc9886389c8bd244baaea7a7bbab874c5f257c28a5b6510c2bd773d5b740be046eed273355cc9b9863e6ee175a12c3ebfeba37bc4ac3d278ffcce" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/136.0.4/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "e2d7c8a8c01db17b2da7ee1cffc13c3792739648e532603ffda17cec15e9cdf6e04e910229664637bc66e8356a25219ddbdb311a03ccfab1240efe652aa519c6" },
                { "af", "87579a31ba872efeb48fe2a8ec066a4f25d565b2e78f90195e0a923e8ef36f992069640c1e09752796421c9258713a603ef8b004500a1370c117e0da9c244450" },
                { "an", "fcf0c6361a5988ef4ea926417c62fb13dfaad1d329c9d68bf56ea01ae50ed4a85501e90394dbd7bdfc58f49168b5ec8c3707e54ccc76b5810e1f8d421fa34db0" },
                { "ar", "7ce413ef08a0dbcb086bc7d2e88830198aa225fa39f93ddda2571604839e369176869097af62ed6313898455b8446c09b5db68ffc238b09565f7b91ef35fe688" },
                { "ast", "e9a22f0c8adb47e1aa7ce5dae7a8c2d4ff9c7962559e4ea5c38904647e4e922a7e80540953115663e03e450012de239915325cee39dfefb09d7636a5a5669060" },
                { "az", "87b0957d1a1bdfaa96affd6fd5c851c7a66706b7c14b59602edc1ffb299725aca4a1258c45bd9d37a6268880ba3717e818c41f94e20dce22b8184fe4770198da" },
                { "be", "c3fa18e98033566b3b973527a847167b9b598f06073ddb7b3707e234073dac252505d1cf4f21d80683ca35ccea9839fb615f9b5a617e1e6e02bc9be7ac267ff1" },
                { "bg", "9df70eb5bd493c613faebc94a79966b831944c1d3b32323f46e8281ca9c2166283652c26e08b5ee8aa16ba5ba0e2302a9aa06b18bc6ec61aadc7b8350e794ea1" },
                { "bn", "1d02b50551a89ea0b61afcd54864c468bd5138a5846f502565a281ae3e185c900cd368ed352727a809bf943573a01ec91b268aba0a6ee7d30348aa464c84f0c0" },
                { "br", "804a2b63243df2a377abb4c58dd2cbd84875c98182c56e176f7e50c6910a2a1b8a41a6535d378f2f91795cd9b2705ab3dbe8b44f74c4ed0d451d825137f8d706" },
                { "bs", "dbd031d0b2fd8cfa837b8cd8ee239ac4e97e1050d0f0a3be8e4f32031926628a7318dd6192521a5135e8c123191fbd8b605fc15a443213d5283644c4b4acb391" },
                { "ca", "ca1683e59ad9ff8e4e9af5129dc608b76695892e3ad170674416a58edf6fec88ac11ef718f081b7093708f8a1a2b916d3924f8398111e7c76d938060330d0461" },
                { "cak", "a80073a580d11e774aa11e9c76e32b50ec97ad8c0081269066fe64008cc357da6092f08743ec48ae4a721636c4397bf51a5b6273b34012d259e32f34a43feb8f" },
                { "cs", "4beb0e7e712f38ddb6ba9b9117017a5ecdf46106e7a5f074abce3fd6aabfe4c52444b9e3972846a1ac872ac79657d1d140f6005bbe18ce9ddce1f5c15be7df5f" },
                { "cy", "d7e4347dd65a090b331cab25a7601471e20c2034c90f2ea353bc37244c8b89b2c0748e2ab8220aad06ee27d6186a7fb7e33177064ba50796f43cc25f673ab68c" },
                { "da", "63f4a8c4b2204b9e0ef4a790846774f5026666ca7f8661bbe35100a565a1079a0b2392f55fcac22443531dfa622ff230eb7424d523fc00bf14887fe8fd62f349" },
                { "de", "3a8b3f1883d3e369331073856c337f00880608af3a032cb8dc2220a23319a998f764ee41f7f16ed170f80103f4f2cc5a749bc264f767fc354261f9f481840862" },
                { "dsb", "cb60f7166c76187fec56740dc699d084747f0cb3c7e4fa819718f9bc7fdd460139bf2764e64c86f9dbe8802eb05a6941abbc70b024c7172e554b4bb8f1efa4c0" },
                { "el", "dab728058425ef8bfea8e9aa43bfd4bf3eea82a611ee477fa707d195c72e2a91ecd15b6dcd72a49608b6a7a17cb173ea1c24a187ae40c82f701b01716c8452f7" },
                { "en-CA", "256ba7c32859e9cf820e274cfe926003dee9d3c372b902a8bdbdaadd0118e7044add765c699e362c3f02b8f376c94321e207b1b84d0602d4a75ef00c6f288c2b" },
                { "en-GB", "763369cf9f6f4214962b70d485697a9a23f4f9b8d863e7a9667d74512744267893a5c554d88fbcad523d82e161c370bf0837f8e2f975eadde69beae1bde08658" },
                { "en-US", "71c59c3c141b4e027904081991fcc8454dc6d9a25c08185838b0befdbb2f86deeb916c9b816fa85a0545a3380c45fd369dd09e651c18c9891250fa27e29aac16" },
                { "eo", "4f28af73e969e3fd0b4a232069d0357fdb00b99f5a4babe35c42759d9964b0b45ac28fdba339a5991a73358b903737610079d51d4b958ff2654eeb46cef1341b" },
                { "es-AR", "9edc251953370a9f9fb232acff2246aec02c39e1407df11821ca1f80996bb24ad55e3880790f701d380938fa750eb16f4e5ebb99e3d277ddad74b2fef1bb90fd" },
                { "es-CL", "6854b79202234f87986ec4a1c27de267913f185f0b5a8c8d102999b299fa41ea1042ae31b7785c8967994f1e8b71e6eaa3eeda7f5ee02de933e43b7a302d8f32" },
                { "es-ES", "91acf417fd8e0727602ef3774ae56999ff1707bf42e2de484891b7c2ab0da6e7a9a99b07d26715a3748d1fd35c4bda88fd58af5485f401feb8788ce6b6ff67ab" },
                { "es-MX", "6c707dad91607187a6b3adab2ffd82b0b963154d649703cafd710c92780e4792d864d177f148d9e74b341ece5a29e3155c29239635f05a1e2875373d189c6c5d" },
                { "et", "dc9002bda736080d8e5252e8a91908e177d0a6a5f94a3ccfc3f292a1bde36822a7b8da235dab9e645eb74c95a0b92ccc9659475916bc2f62bdac6cbdbff96800" },
                { "eu", "41897bc7d48610139be9c8acb81c6cb891f8bbf6b0cf26c8c0e48444918f6ea8770d836df1513188c5345c986aba4cac70fa43b4b6a6df9c7769523725481aaf" },
                { "fa", "45a5d084a50318f3e63c136c83f857ed628558931a0069c3b7317a73da12fd6d73d3c7bf84dd4f2c3a87e63985d7f85242bd46821c80a4109575bb9a48870408" },
                { "ff", "34d8a3c5f19682baad61ee96ea437cb0097db8cc09a35e14415fa5ad6058faa58bcea5ecd2b0655e6db8f9882f6b83a4aca1d968f7888b74f6d0c7b8f9f6b8da" },
                { "fi", "5f76dc860f255c4445d67a1c455007ac1b04577d82bda7359020318cfb1e6989e805e8f705bad5e79eaf071084b5f2046e4c9366ab5311ef1d5b20fba992e245" },
                { "fr", "0f5d0f6faa4a8d9c231de3ebf0200a4d48fc8cb492b2006e069a4d909d4a708c549e86f5517c104c0a3f1ceacb32fc2313919aa865129eb707d2cd919f1ee22f" },
                { "fur", "305c57f2efccb0d2cddd6534d02234db8d26b3430275027d713d9edb759f8efe0530472c7596ad37dd02c2761d2f4f579eaa071bdc9f5659ab1236fdd967df8a" },
                { "fy-NL", "30b5d0baad60c16e5b6e54e82f7731f3cb7e632c5d92eaa226f2fb08a1962adb6f27987431a33e84656ac28d662694a5c5fef7f274b89472463931ff4ebdc60a" },
                { "ga-IE", "c535574bf89cf28dfe50b8f8c38d1679fad62f94403a722210a7598393312dcabc79ef8a5d7fe297366642c7ddc9d20cb90c5045007940a25c0d627ad627d6a2" },
                { "gd", "b30062c650ebfeec56930cba732f21be002e2559c83b09429025762edafe34cefe94c3a5b6148ed8e745cae8150d4280cc34afeeb9b3affeef0342c17434677d" },
                { "gl", "b7c64a62bbb92980feecbd09da6398308524b8d686a0b4b3f89f19f52a1d5793f71fe91bc33ab682aeea5f6e777684407800ecad8743dc16c4a3607dedb6ceed" },
                { "gn", "a6ffd063c19b04ca114c0b75932dea18a89ff989fc10e5b8fe1999434c5b368989e7910b8664b23bea72d286916642d816dba971c4ae152581c24f159ebf27bc" },
                { "gu-IN", "bacc66c3dfe4b0021c5ea96f7adef42e045d1e537b827096fef925b9607b458b2ff9dfcd64d10081e52f5e578c992bc7d3ef15bc28816f62585ab9199440d336" },
                { "he", "c61743fad13ff080dae7a086caf8f2eb64df25a806854b04458472c4b272b666c5433528c4e34f633963c6b9cd8c537f01f1e733cd20bc9334224469fb231f5d" },
                { "hi-IN", "25d1daad23395d5d847b205c0ce72b6aaa56a526627f4e53f2c2e70c99402057ac2e1e57f096c379b8ba675433c678750f002ccbd7173f8af3907e27fce9e27c" },
                { "hr", "b067db1cddcd42e8bb4ff17b7be17c80f7a059d12f2ebc70f1dc5e17e4b939888ed612bde841d661828470bf44d2e8f72ca86bd5a9f5e02e51b3b4a6e2067ebb" },
                { "hsb", "7f4470dbe46914ef23a7da90779aedba423c424525da4cbb1bf36c09ec72750448d98cb6caaeaa29603bf8149987ca541a627b8caa74ac1bb05b51b817a4f8e6" },
                { "hu", "6da9d387cd5004f73279c450ba014c712b6ec1f4e6e57fb35fbb2803c5257eba824d98bda98ba300ecf858f0aebbf1f56ae2d4a94142a275371b8766ef189eda" },
                { "hy-AM", "cbeee5b5b155396ec5c46b8e375d5b2e989c6a72e4a7eb696408aff168c00a9fc2b96c365e2d011843dd0613278aca6acdbfeefcfdba6cada997efbce4d19e3a" },
                { "ia", "a37c0fc119d0dfca5881b0ee8e6ec2784a97a52c8ba0a42809c59443af9314324416ef6f1e216e10bc7b19a1ca608e50f89fe063ed9651f34d748ede2b4ee21f" },
                { "id", "fbc28f97c8f8640a754227ea182764d5ef338347350b1edafbf9dd18e9c7755a3303b91f3b754b7687a32e1eb2d025dad4fd857bfce9aee533fa085fb77bc4f2" },
                { "is", "27438c1c1949f8a822f8955f0ec6d522717ff19fa12d56c5920a46ea9b7a7b71c188c19201df15b9a812e3319c63d6facb643fc9eb09d4aa360cbff1e29a2518" },
                { "it", "9ca8fd1655cb6af54b19b7f0e632b8006cc6a80f1b9d6931cdc638aa8d62334f9fc9cd1599d9f90b212f0fde22c90233fddd9169c7b69a60c3970c467b80549e" },
                { "ja", "a048b27dac4e6d7e01440fcd28e7d5c5fab7d294a7f2a3ea70c101bd8f73395350e0a616acf2cf04056c0f9c17d7494a5e0b798e931e54aa4f1b36c2e3c31f8f" },
                { "ka", "6c7d29cf8531092125226fbed3ee8dc819b7e92436e7dc68da160e39b0786d485b4cf9782d88556ce6a6a0886f41073b5832a32490dba7f53894c0aeae262bb9" },
                { "kab", "4e7ccb887a8e64b25790e388ac3a119189d5b8627fd05191ecc84ade56ba45ab1afea91b7dfdfd411ac13ec471a6c521bc927d4e1c5c2564a9ca8e69685f057b" },
                { "kk", "24fea8c8070d4f13539f590d85f1a8c50d0ec7d84cbe05fe806e158d9ee9cf5a3d59fe5b6a5dabb398d1462ea3b0f76ceb9e377e9303d3df007af6fc75a54b95" },
                { "km", "c4cfcb838b0258c346f39f3cb726fcc1d515a08da7e0d019303f787a13c4a498ab5d3647641da1784815ec2572810c4210482f426990148a7ecd938d37d45481" },
                { "kn", "a141092bed63928bda5c69d17bf4291514ee65a3ec2cbc26f4a4fd3a6ad7d4aee67a2dbe94307abbe821c4f0f42f0f3e29afd21a47d79246aa2ebeee95908f17" },
                { "ko", "165a851995d70b13f36555d4f3790a45a7e60c95236a4cf6cf56f0ac05f6662cf924f95f305c0d9cf58bbb3333a64defec13b8cf3f97ef1607de3d89dab9c94f" },
                { "lij", "632c1fcbd44365f539d907f9c5755b073d7688a894c5d0211671463e7b59bda909bae393125bf623e7f5f4fd1f307bebeb9a662f04286be0959f1385d77fe93f" },
                { "lt", "a9f82c9f04f548041430c05d817bdbb9d801e935d30f294ac8a6bc0f5e4da84e14bf4c97d1c94fb4d21a3759277ae172223b16df8c86e32ccb55612a710cc157" },
                { "lv", "d6f905315cc594ffb67dbabcfe0e149635d7d23dc518e9f1a8601f7ac263f72b51498711e06a18c5d304f7c81456fda6eafea5ab7824aab5a631f708ac3bb53a" },
                { "mk", "4848cd942f61e9a6c943a10a6919c78e09980ea5cfd595ac19f22d6e975473ef6e90a9d012193b501f01681eef973c6fbd5866bcbee9a652c9c779bcf366589b" },
                { "mr", "766dad8e007715a874567ea122143492a677cedb59e9beb93843d452b9c8a29c6432389385e64e8662cf4b4ac8489317afb5b296565fee2840490fc10d1db8b8" },
                { "ms", "8e647b4f1d2f12faad7139600a45a32aba7641e34cc534f8c76aa93fb32c46d0411797aa29fe86308954eb7c4215427ecee8fbe72fb40f3d3ba5c7f3e0d5daf7" },
                { "my", "4a27ac45bf6cfa6cb9e040f81a080ee477b56c59be0fdbd0131e7c63ee46b92365492000d23a3fb24cbd10093de3f7272aeff1ff0b6c5e74bbb4423023896c0e" },
                { "nb-NO", "b53175130c791583a7f3a62c6d401e4b713c4a81c6a743247b088b7ecdee126b0793d95f86ccf34eb3a63c0924704f67b85b3cae48c979eec427eac20fba1fef" },
                { "ne-NP", "40a83bd1a05187917e0633429939e90954c1b70c3127674f030b9c3b7f76ef60bb5eeeac3893757407040dd10380aca833619e6ed82890b0655c66f08151dff2" },
                { "nl", "e730acc7a559d8573face352ac8eb3705e0fb687d538ca60de5341a28bff768617fcd855b5e3ab716917ef854dc43c239f1c8e4434c3021f8c2101260fefe259" },
                { "nn-NO", "d228e15fe1959fbefa502ed065962e4336fa64685ce8538fcb7c22ad2d9305f1005b5c295001fe5a9e9a3c48723ffa99c35738abdbafe5de1df2a848e6710ebb" },
                { "oc", "97214eea526fb25079eb00348586f91da5317b6b2a0891f9074e690c7906e19184db248f68be1bcbfc8d76b35867fac3b0855078cb2de0d42899cf56a2a1c861" },
                { "pa-IN", "863e729548be007e6fc7731d8852af501194bcb89f6f7c7485d6ce02d4f129069f6ca29fac15b60a3374e7ca751b92a0506cb85df50c6d8f0061e2e82ec07818" },
                { "pl", "681d85e453802b027bbb28d4cccd7790a1491d10fb7538410320481ff316950beb8bfca2cbd959b315f3acee4ec6836302d4b2af86db9bb9d131a4db8859b34f" },
                { "pt-BR", "75f2447a1f441f64ab13860b6ff2fb8f50b6554c754e4ea5fc340553590b15d55c2e987ab860846ca1c347a4c29ce879bccc4c487a5153469c2d5c447d6bb545" },
                { "pt-PT", "8c617a732936c2df173de05c10f50ddb23630be55859a7823e94973efc1ff0a5e0713fc920b8fd2acc7e724e2aad9745e6262106a9d6e9034f41a16df54789f8" },
                { "rm", "32f9b1cf9d625cbd4661350d0aab9db86fb5ce891d99d035cbf1976e2572d6514e501c532cc82b9408273e6bf13e746b84d97834039850b7fba0af7558912f55" },
                { "ro", "c3eb64f31e50526f5a249dbdc94d3bd502c39422c764e741e364e325f83795ea1f9a7f65f85bb1a3346b3fc6bd8e4c25758e32df14257f29c7898784baa5de56" },
                { "ru", "7aed0074baa0f9f36a3742e967f554daab60af271633d00c880d4509fb513ec6cd266fa32536bf31b3d32daa6bdb166f7d68c72a4062fe6e6c35ec88ef72dbdb" },
                { "sat", "ab21b48bfc78dd25d3b5cdeeb08096b8d6b71a643fa98bd58dc3855e3e2065826ba72c360d285cced561b7cb5caed0444b184205d7b589639c5bb35e04e7e8a2" },
                { "sc", "6b28e093439f6e2c724575313e4e304840e5e97c2d22ee4085866a728ddf5289e24d102f59c18ec53d816917c94794f5ddb3be6c76326a58a2802cd6e4a16d02" },
                { "sco", "942e767c14346231c84aa1b8a9e3cbe6cdc7132502c6b508c28aceb589b41251ebc2694709c95b020a0529fa5aac4fbe99846b6ba220ad5006b6ca99d0c3c361" },
                { "si", "a76fb4a21064f53c47a0234c0d35dfe38cfb736ec6c516745a97123cc94e78c9ff89d31eed44e63d28b0cdb7f1b76e5bf6594bf6eb1ae489e74d0fe1d36712b3" },
                { "sk", "6f086d3007524abadd858cc532eedcbc386d563d364427ac2c8421e566f4f62dfe14fa0a14ce762870e0f904f1ea748ff4c226d2c3dbe5684088920a4f74c0d6" },
                { "skr", "26f062d6a1fb4868da5c072474252d39653588950263ab63a8fdb6a2bd0fb1c90417fb31b000cff1ed7f611fc203a542e8f0029480c96281c16d125a4e801684" },
                { "sl", "ca193d03cc151e6d1cd8871e174ede473a204ab1867a6788d83886d040277f89f939f1942f9ed710f2affb781dd7bdd80b05c8603b3751cb163e11c8307fd120" },
                { "son", "7ac43c57eeba55ca2fdc3e07a00d418b90e53f7d4c8fe5d6f82c6bb5cca9710a576ec20de083d5b0d9a26e74533354edf92691d3d8648b6364f02b517dd21a39" },
                { "sq", "b173380a886346da31ee57c7a002b2926d2648ab1d05fe4ff1a33744376672d6e3589fd0b1259b844bed3f609f35db6792740accd8703f7525100b80c4e71100" },
                { "sr", "3693d2c90a6c2dbc5bf72e27836030b9ea5f9623551f588dfc238fdbff98c3083706a50e6ee4d7a401e61ddd1823ef9eab95296470e6598ad3926f20d876107e" },
                { "sv-SE", "92a7831466f997f785332c1310e6b05cfbe87e9af7866311bcdcae8c2cbd757cbc4fc598ae51f31b42d44c55091748dc3a2326bd200ef994461a1b103b95452f" },
                { "szl", "8765fee7c431ab1cc03a0adb6b79417f70a9266971c1f4ba8cfa08ba0aec2e4bea65fd4470a478ff7f93581eb367470bc35cb4a418c6b22da93ca5b9152498a4" },
                { "ta", "d2366b3f28e8b8d820d0716a6f1ec05b5de36c28b04dc2db66b8cca953a351c359eb36bf24807277cbf3c0c9d07507151e596fa6ee4e24c3c68212ea9324d878" },
                { "te", "12c1c5a2ff453d0ef4a648984e2ffa39f965a5caa3fe5a01850fd245782bd772739cfe78e0d5935c7d9844677bdd7b63a95057a1261a04a0e05144cce048b398" },
                { "tg", "f9c8a46a97b3da1c31d40af1eb3e86ddebf49b82f059ca4a042a38442a431d5219023b25eea572b0f2792301fd6494659feaee09effda36c40e07bb940e84b1e" },
                { "th", "9ba6ae0bff18e4ee26ce2af4ace789a81a492e07c1042d97412b78aef65270939fe23eacc2f6c8ab56bbd18ede486cd4ae54cb07daa6970ff85a7dc9dcd88685" },
                { "tl", "1a24d04b542df391b36bdef8ab3a854f6fafa8a647f62389e1e21e4a93644cd10c7388337ace527767fac16a9f6ab39d238b4741bae1a7628513066c8fa44bfe" },
                { "tr", "21a41f4b26147b3c9323dd2adad7a0b83312dfc95bbd011a04576f48cdb1e59f14c24f65ccaf4345f6f965767a5e7f2833a49bfee332d6ace9b8139b05798b33" },
                { "trs", "bf4e367ed4e5c504632c01529ae1ec4eea9fed5b475756edf00b56cc5ad610bcf8474a2bbcb3748b9fa90e299847616d4f9cdf6f759a2a46d42df72aeb105bed" },
                { "uk", "acce09d90bfac191e7522702c3848e5c25af2e24dbafa7c4508edc1cc9dae4ec78f525bcf000b2bf94d58dc0266886311d6be2004c27630d5fbc5943037ef6e9" },
                { "ur", "4dd00e7546260cbcbb983c08dc33cf62b091da809718afb020fbe4d21a81d6a644c1d9dbdc62e6df4cd155448f4b0299650a3e8aced67bdfe69db9aea446fa92" },
                { "uz", "a70b86a7d9a474fb88ff2739ab6207e99e64b93741d5f012c39ab2bd5e3a8599d22cd8dd05da148f5e615bb57cc5bf7081f9635cd4b60b6345a4d545c08392f0" },
                { "vi", "6fe2f875985b7f74c853da035ee1fb61c5605a6ff200311fbdd2e7894b428ecbee1219c98eb422a1ea3027d8099c5d8710c5d469fa1bb28af1bf1f4a8c1d385e" },
                { "xh", "8f941bdeba0d878bb342192cfd620f464926ddacf73a5f8e302b734ae17e9583485b361dfab8d010d20d59ff728f034b91b0c470ba43fe61e7ff80c7ad706b11" },
                { "zh-CN", "d2e526da0cf6f2b07bdfa60afe4868a14b2629ef9bcb80a7967762274a8aa57af2d6be9dde903519d37f6d699b06c1301ec46c8c3a019c0155a44d018f7543ab" },
                { "zh-TW", "fdc073251a3a68c4c90daf27476a97ced577a52b37e0c931696f08be0a89997cc82cb7e13161a1f2508dc3f0779bd1332862b314262dd6cc1f8e8fd41ab34666" }
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
            const string knownVersion = "136.0.4";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            logger.Info("Searching for newer version of Firefox...");
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
