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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "112.0b6";

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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b6/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "ae1ca3e6a1223ac1e9b3e0a7788caffcafdc6900dbe140f3c8b583c87caeaee6dc8ba41f802681fe621828d7de6e525b700ecdfe218244f8587793de67fd42a1" },
                { "af", "d1f2389f49edb0df2fdccd7d0cdf269850d3688603db7e4d206c867048634b064cbc3dacdecb3636838f34567f62b53cf159d0197ea317c3445226f1cc334149" },
                { "an", "2702a2a8588d55bff5cd5ac69b7828f75a35dcdca79597d5db235f2ec97f7c26b8eca03ca717e7b1e0c0122f6ffaa55ec49510f7cad5aea5f287ff529b5ce245" },
                { "ar", "3651f2c59dc12ea0cc807104b0d0e8ca37864261286054245b3bd0c1a5fb48b01c1bfcba90d366c3ed36137c70b25400f99646af42c4ee9b91f6c75e849699da" },
                { "ast", "4dcd8cf133f51b2a528cb96fa636eda45b3250b22427af3c8681e66555eb5741fd2733617bb337ca4919dfc7352cc2aa80455356d0302c329d74de16be92c01d" },
                { "az", "e10b43dd3e83ad90498bdb3553b78b78f08aa0f7b1733b0eeb032f86f2d977861484567c03a1a1aad119e0d82d3cb40b4e6191eeb0193700dc1b7960cbf7d972" },
                { "be", "32b14017cb00e899e1f087c22554d3fbccbc811febca926b17393d4b7d227f7f2f4c53ed2d9e16e99632fb4ffd896ef293456a8d29982d06fa436a8c2bf33f3d" },
                { "bg", "b53c26e3ffe9128fc8e145d3cfbcbeb323924eb1143789b0fedf94e65204a35e2b861c1b774de644a826aa7063b356bea38ce3e63edb08fdd2daa433d2183f81" },
                { "bn", "ac2657e4efe8ae6e51d927b5903b375db61aa601f3f95634b9caa2102f57aa23c89df8b8f2c18adf0b6c81c95d7bd7896f72aa22d18d432979ccb4e72de9afb6" },
                { "br", "0381bed071d8bc79cba62b680895bce2f0f429c0b2e2a7b1c9209f9202457cfa1585e260428f81a99d1ad7fbf19e1054433b487435c33287a51b6c259be01f72" },
                { "bs", "e46d593b365a16c04c3aeb55f50d00af6100297e3f85b781ad3f4271f3e2f585b3b5ca82b8666d0c200d8fab92fa098c2219ab7e7b7002a8c6387796260bea05" },
                { "ca", "488e7d511385f083baa33e689e2c2c1b99abc1a4abea48bf0052fe4fb7d382ca5444056c15e44ee388d4ba44e2d4844d7421e2bcca59c8a0ca952e2af92fc6ea" },
                { "cak", "7c505441a32d8d61fed19f774025c0d6918a1a2ac75a22802f9e8185d97f87efee4aaf33b16679e3838684618fd720e9ca586fa36479d98ea904580c70c5b359" },
                { "cs", "e6f47a1f5eaa867f3be2edfd98f800ff18a9724d2b7b022a7223e9f1ac12343923c91d33a576d82a2fc53f62c6aa7553f2adad1447c0a8dba4551dca85cb1659" },
                { "cy", "6c00d92a9ebd9b3df832b432d509768cee1475dbd3ea456ec35df4b7a3b79fa8bcde556f704362cb329d4e732b7ee67db9ba6e285ef7e08bfe9d9eceb3d7aebf" },
                { "da", "4647df5c23e83d2f41ef8e175f866a03f9e83f9624ba6ef7cfdd040654865013ea47c603150b0c2990264ffc41dc6cf9a6c61d9e84bc913134e00693f9ef71ab" },
                { "de", "48d6f1787693b3c41cab5495b0de542cea653c603d1d8f89d2268a9143f33eb10203be8bece18390e936ea62d8eb5a52d064b7604501277870a1568cbc3f8006" },
                { "dsb", "a0d2f0084b9dbda095446251db8020f7c355a98c9e779938392823613dbe32a6e196630647dcc8f16b75365a9a8333dfc50985fe4d1026308afd22ea4d3e3d46" },
                { "el", "3aa59dc3450379e4beb8ee1808693dd091d1984825984259539c82cd92ac446fc73fc533102e907ee9e995794a897f9784ee5426d48757e31ec8ee43a7a2f740" },
                { "en-CA", "79042dac34a5dd64b8fc0ad58ba55d4895760efe004e26eaa358f6ceda2bfcd7eeb63fe235533fa91c1689b246a1e9e15254e8e95f9534c70b0545c361949cea" },
                { "en-GB", "64fa964ada5fc96232f60fe9e60b10ee968a134a7c24b61a4566069e0d44356ebea3562c0b8c74db067855599c093573ed905773654487ff69729fb4c4dc26c9" },
                { "en-US", "ca6369f1a0415f93bd8d31a8cb3cf4f1dd71867fd27d5d1aa30310dc1db690e92975cf3a16dbbead06d74a70625146e59600e6c51c95db134607fd4dee90811c" },
                { "eo", "38adb56d5151dfdacf15e06902cd19a3cd82f486120e8751e50cdc515a99d266fe7726c1fab2246993ad5e5b9179a2956985319d1761c9d82071ce86bea8267d" },
                { "es-AR", "a63d3441709fe84c3094e953343aaecebd2246765e30035ec1dc3ca7913431d343c744b9f1e4248c027efd4822252d96bdb4cefda91336101d6b2f00e3367ae7" },
                { "es-CL", "a6e0be347216c91d5a802c26376d41fe5bb8f90041c71f896c4d838210aa60ca9bb08ddac12018bec1e03d0c9e7ab4b83f248da8180b0e099908ee9fefcd8290" },
                { "es-ES", "8fcf97e2dac3cd6406038b64e30835c4ad3ee54c7a2dbe798a2880ab0362938d460967ec1988b5636fe71f26250f7b690c419ffdf42783c0abca4f754e33de30" },
                { "es-MX", "61b8bdd6ded198fb730cc1145b25332661a0b228a8ea6f0b8375f91e0d0f1020ed889e0fd240883e2a988f77646f92d14fd1c0ae899649e77984af3dbde9562a" },
                { "et", "b924326880915580c9cbc6c1e0f9904d0647950b3e99820f12bcf47a07aea9f9d633b6769d8e6df6444f4683e0e043216ecbe633451d2d18a7b7dd662812cb3e" },
                { "eu", "5a708cc7e768f347a05a5381a93b70e3360f1f80f6f87c6062618f0740c5e590733ca087138006f6891c4a42bb72f02278dad7d40926c8ea4d231333d77e61ca" },
                { "fa", "4ac30a2b892f32b4de227665275429cbf559df439842c75076d5b3e5ac2c890d4ebb475ecc4956f33d71d7ae16d3520417f5eaadce9b2432e865e9f98f78455f" },
                { "ff", "ee5f353dc29cd9cea35fa0878d323ddf8afe10565e52be0d766e99c642c25d771c048d8a1e766a0b2fde4a81be745e9d33521fd5aae7c99785f6fc1dedef9e5f" },
                { "fi", "259a5e541dfc31c1a4f4f4421f59614ff44298047e5ba1c5e0270c8491db8efa80bf9591ae3f38fda3f7bd9aad7ebf07def1891f518b53a26d83b587fae4e4e6" },
                { "fr", "bac402bbb3d61a934e933d784ca7b7b7a3ac8027e352aee531cc0cf50b113ca80223d2d040ca06c2189bfd8ee9631919078c897a1cbdbe629501d31e49469b8b" },
                { "fur", "b04bef791803cce8f68be2bae12f24cac7b692397c375ab9a479b67250db5e92bdc2284af844686d38967b4637c8750ccbbc0885da934379f7a020fc677c1f7a" },
                { "fy-NL", "f551ecaca8a0f5d898394d8b36784f6f0f7139e87d11bec80c7b61c6b6c6a222b26de95effe7b1ba754b57049dbad81fde15c0343dc616c2e0b3ef8146c0746e" },
                { "ga-IE", "229796420e3f8c3a413517071e52f6f8e1e8479b236995419ba2ba99254d2c9e65b4a5a4a35a0a6319cdab3cf48f3a0f9a984f682e29d60827ffc6ced2a3d218" },
                { "gd", "4f7d76a718ff0cc65dd22758fe5ff869ed943979da2bb27d4ce870a0369c3cdb54802958f28d6b50cd1247d9f7869b008b0c50f3bf591b5deee6d92391fd21d1" },
                { "gl", "bc7b6bb82849babb0de6093430ae3a2460f78cb0db17e6dcdfd7164b7b9e0a241954efbb2d54d6bdeb384705df077ac028417174fecf06ae67c3bbb97569dd65" },
                { "gn", "a106c480c2af33d3affa2537fc80964dddf4e6d5da5005079cc679b58cc5e34c3b1ecc604b3170112e2268a863287bec2b94320124401764ca0f63ee71057baa" },
                { "gu-IN", "d2071102251c3547bce43d943a06682f59e1f0dd9eeb54196331f325e76ec970645a5c62770d91d816124d7c7e8a3092fe522f703ef322fb439fe8e38d90a915" },
                { "he", "786500b80e956ea1a45988da606dcaee8c20d3780ad7c0f8b086ddc9b3418828809938d52841704698044c7fbce3521d826e0b39df78deb6041260161dc16c14" },
                { "hi-IN", "c7c84a316ed584088940a579fbc758393107944b09d8478bc0a3e667c43479ba471c589532e274616b385cb6db552dbe4a3963fed2fd95eff4c251ba2d48a6ff" },
                { "hr", "c5860c802508bafaceaa784031ad57cdf3d3ffa0486c6d43fbacf93a1168062e4469278492ef0f02cc5ac493f2dfe84dc0c43d6bbb28a2e4495369cbc69b59aa" },
                { "hsb", "62cdbc9fad6533de19ba96debc0ae8bb14a11dc2a77fd2ae2b0d93d9e801924f7f5c9b137da1b0887ee49fba498281bb4d01db849c93c384642a9768a3ebae31" },
                { "hu", "eadd9a8ab20686c0993ec716585b91cca4da361685f47adb4071a9fa6e34e8018f6f6ca37b0fe771ed6f2871d6348903502e9d919ad794c66abb857601edea0b" },
                { "hy-AM", "ccfe014fc4d4f05ff1b606012fbded3c24a7b6ac5f6976136f46bc2ce19f4800ea43ec3078e8a8ea7700e14e2d196b538a855a8ce0a3b9177a6427e98a3dd83b" },
                { "ia", "be0d599b80c270a1c470a9d3473516610ad38e7ebc504832a7ff87037deb1c472c3fc4f33534407c0ff1ce694bfe630203e5c6453e119a34b8ff5b3b72b75333" },
                { "id", "2ee84248b9df7af95dd4e34dc873544fff7fb9efb289513307e129403a2db8f5eeecc9e51f33c9701bba5725e9681a6ce906cf77fb87a7f10f78a222547bbc83" },
                { "is", "c29a4baccbe84938f4e2591bd24f677e3508245c7ce02974eada26e6f1f0e3c7964cbe81c208146a0835b57ea30dbf4b75bff474265d198fc627f753bcfda124" },
                { "it", "747e4cd7c4b7ddcd7b1d55dd5a8149a544bf97199ad5f19f5e96508eb2cf44c5aa35c70997a968873ed82d8c792fd196dd59cdc598a7af584ffe39f9d41cf259" },
                { "ja", "483d1209fdf17433975961240e0dafd13d273b5a360dd4b698f79879cfe51a042ad94f2eb23a9aacf8df5c0fe334af32516aa7a805b0d4301a7d8c5e7d36d3dd" },
                { "ka", "2c0f0b4db348b17369db0bfa5c5897763e9a688f1b06793f6ed8426242e6b231fdd3cc92a9c3e4808ebcd0702751c1eb2974c8bfcfbf8238615b36c931860117" },
                { "kab", "34855c441cc88b94181fb62cf0132a4e41bde0029bc71753de90d3832889b1139c4108f2c8521e8cf2a2e55da136a21981ea9dbb49d69880fb4ac68eac88f105" },
                { "kk", "c00d62355aec0650e3f75ff91e93e5cb914832c13115c7744c352e9540bc73b94f9653c44d63b5dd5544d7dc42bccf6e05999bcb6f8b4865e52be3faf9128c56" },
                { "km", "23a2bd9e05eebbea2d9260ed6276af5d35d14e64ba9b13c0d86e4cb33772571afdd169ac800cd27156938804e6537d19e5b8e6db7b8aca01e3196b6f74af0cff" },
                { "kn", "9a8e150686c485a8386d52461cb1f099afd11412d61caeb72f1413a0495adb02e3c9dff3076b3616902733ec37be433b52a3acd86cfbec3c62fbfaf22ffe7db0" },
                { "ko", "b34b20807a54780ab7ccf5f1a6b43d037a189de5e065d28e4108718d74e97d783c5b1e1fc3c4e5007b83db7d1ceeb9435adc03f029f19ddef92b4658bbe3f3fd" },
                { "lij", "79458147b1645578c51c7a4a9dbfb1b23b96698c553a1992c8281775fe1abc68dc0f47d064119c1f85690f7131aa4eae04a68753823aed52197b39d05e490797" },
                { "lt", "3a34a249faac9f0d0a6666406dca52fcae0d2d616c041edd05b7944c3556d42c9d0a1d77598489cb7b76c5b99727c89a24f3f906bb253a388957ccc8a47e649e" },
                { "lv", "a4d0116d8ccdcb8e3746a1663ccb5ae04d6c0a96e722d04a1b9f3d9f9733b8434f57747924c0ecafe62bde130fa2b53d4fda7ea371947f0240528a40783fb216" },
                { "mk", "5d5a3da3ae0945dced258719bf12dbb2463ad4b14fe5dda3c3724f40c13bcfb7312c0cac7e0ad78455fee23636d577814602bc78584ad8d297acc915b4c409a0" },
                { "mr", "1d24b613941c9344fe629cf86e1853fe320886aa0a63df3e3f956dcd209bca7c8dfa7c4d00a6a054ed40d32c4a4d8ea2ee5b22d700db553a86292a746a1bf52d" },
                { "ms", "a410a2ba99a0f229c421a5e31988dc4e25ffe3c84375b2ae40e978d3ab42d9040c811dfc89dc50326fbf0bc59a7cab4a1d83674221486ad03439850cabaf43c0" },
                { "my", "1d3d34e1a64e6010924f71718dc41d48c24783dc6fe7cbd0c2390340b3f3d658d6a0e57dcbde19be62ba95d05913172953b6e27f1067272bce68d320d62066d0" },
                { "nb-NO", "26716d02d123ef3ba20096d777b5102c653997359a0eebd713964866d9be6bd175205df29b856fbcf451c675f7242fa56207ea620aaecafb9c2bb75cc680445f" },
                { "ne-NP", "e884ddc3c7a2a3b9836e2dc7f47073f868f0dfe09f70c27bf0cff52011ea8acface4020b57f4c55bbd8cf7aba4b56ae083e297c7ad3cf507ab12e5665e0bc9f3" },
                { "nl", "c4c1cb7e799628fb3e57e4ad2f0910578b7b1f57ce05e45625e3a2b1286af7b1c714879b8084a6b470f92a15dfcea782d4e0a701d71aa2c9ff81bbc890008942" },
                { "nn-NO", "30322ed2359a7d6b0dbaade9a29e13fc4dd7be430aa5670852c749fd5c42ee56006677584794825d419d15b04744446f401fd2919bd9046c4e80c956f8a7fc91" },
                { "oc", "733234a6b589667df3f8f9693a0c581094235d9fec1708411da592fed1c56d81d5536fc43f98fe3500f07bb4542ef8f7f4189d900ed6fd53b194c2f82429cb47" },
                { "pa-IN", "cdfc3a3a824d69d4eca416c4c74a14aa889858fe1a02f25ad8032fe4c3d82a0595a1df031d041e8a335aaec31708d60cde1f38ffa48184cd289e2472ec2d6753" },
                { "pl", "bf5d854cc95b1b12bb1d06ce8bc49db284ab4463f736d1349ab93ad56f281cd86482ccbf8d5122fe202c7dcd29173406b1cb7bc5ba083859f20d6f43fb98712d" },
                { "pt-BR", "4c30def9c9ec319831d229ec454ed9eeab3cad6bd73b9363ae9223347992fd64b355aea171739deb2a98052e681ba6efe7a6e68815693a886d980320d2214843" },
                { "pt-PT", "5528edb28dfac29772c81090ab0f6f96ce7fb1e04243aada0412957f74fd8de0860ee39b27039618685bb74c702bdde28235a08c8bfac95fc24b8a503e9bf45a" },
                { "rm", "f3165d77b1158ae22a0b8c1ef609e3d2e1c09ea92882865a11aa0c6cfab205cf3232cbd8b9cb2fdb105c60eb9e64f6ff62d498503fb24889925eced0f1e1bf86" },
                { "ro", "bb7544d0bc41874be55673e64025c5617e7a4df9d40fc80e2dcb0c7fd8043d2c9ef0122c28ffdf5d87a3724ac3bf152802703b7454f7f115834fcf6666f8c18e" },
                { "ru", "3b882d6b80ccf0807c8fcfa6d32e0bf9179543784a363c1e26f0128d0b9fecb851cf46c4a8663985cbbbe18c69667b509e279f71cfe9d0f84cb6837af9c92527" },
                { "sc", "f68494d9041f61aae9709b1946fe812468eaf9c0ada1831693d98b74a309324ae540b2bf79f6e606f904f5c7e8cfa102a25d502ee31ba37fb6b4a4a7e6f20a10" },
                { "sco", "9a5883210504fb8e2c6af70cb1355ebe8fd1ef5c5415679a0af3a54e2c62fa4a84948f427537b3e8b49526fb1c8345eddaacb49c12d3cbee52d38f66b7ef7a05" },
                { "si", "d363cdd72fcde90b6ed5b0e09c721953bec27ba633ddb2dd4eba3d00c44a54bf42be89eab44dbce358ede88c58fc49dc5267dbb9931c4dd7dd072bf509bc2fdc" },
                { "sk", "83b23619e2c22fe6d080863bbb6aab5079b8e4477c9524ac2d107373a9abc3a2d0225de80037b8ec5b3e9182d2c52c5775fd7a25c4d4dbd711970e9c4733d0a6" },
                { "sl", "8454da0e7463b380d900e619591284a18ea579e3e9a687b9631aa8411f626558c36eaffc771025763750d890343ec2043d51b2939e5bbd1d7ce34ea52ddf8a06" },
                { "son", "e2de0ec566ecf7db3da8ae09a7894bc030916da6f0cd4cf58fcf9f83fb642bd34ec9185cb3f6c121dfd52a6e895dcb9fb9fa789624228a98b34a7d59cacb8872" },
                { "sq", "c2b36ac074d857f0ccb8756e3025bd035b8f93b6497005a56850ce2b027e456bac7f52e0340acde6b8af07ee2cdc3073b271cb15c359839c321928d3e6e5dc7c" },
                { "sr", "d8977e2951fc66341054da11eb438f64426afaea2aa5d986e371f168911cdbe9560aa08451bdac20f68159af4f88af1372fde25fa4144c441ea519face3a9365" },
                { "sv-SE", "329c9010a8ed7f87d4bb2020d866640ab2068fc92e1b1ee4498256e90b3226a012da31ac49876db2e51d3e45917224c0ace0e4e0f0013d1c9652dd691b56492d" },
                { "szl", "84b14b9b70cc45bb357bd0057eb381bd4d16de4dd2d4f1adcbe71a771bf572c07cc4493be4f3a9c60e91b1a5c09acd0d754af1bb9a100f91c18b14a4673a589e" },
                { "ta", "bb6e48fdac69a0f4309ffcb5a166983d617e67a667a56a577272473f1ac37768c3eede969cf3e11006d6e41730f55686dffee6c81c794e961c225eefaa7fbea3" },
                { "te", "17b77298f3cdc00c835194a0fca1bcd853b9aeb068481bc301fa271cd3d6d559e4c9f725d047e8d499d590dfee525cf08077117bae05c38b1f81d1840461330e" },
                { "th", "316b69aa2cf009b932542be59bbef315cb5d1a6ad1b7f2b9974564b4b4d53ba7ab5918dc63e1268cd9a678090495e609ca637fa87e51d1abdc849adb6d327341" },
                { "tl", "109305811b68fdf3280669b75d9949cc749b288725cd1dacd15c7702c1e18e44c20390be00f449cfded7afb22a47745b44fc3ffd48af49d15075499638854697" },
                { "tr", "9666199721e44087ef2d7284b64f5563a01497f145913dfa38d09f87d0d9baeae66b96faf0b3a5b3ab5a36ae3bf9139444e6dc70aae73281dea42bb390c28fe8" },
                { "trs", "1e0ad22f2af15894954e8e0be6b3d4d5989ff31e9ceef262eb7039ee9a191e7afc80f9e3e13676d0c4c67a335a1d3a5a1822dc9ca52688c0666e75f73bd5f6a1" },
                { "uk", "e12ba3674c80867aada6b796de9df2576901e4cee1af35965301f687a1d1af47c62b9cf8f7bc076b25ddf452e8baf199912b303e918167f42be1829f50ea5e75" },
                { "ur", "cfedd836f811cb3950fc6b3419f89f276bf7e2105bb6e2a8eb935f8bd77938e6309bfec77bd9200c26cc1caa21e62f1317a661244616971bb62e0a0d62059671" },
                { "uz", "befe6cc80410612e11333aec43b97f0110cd7075203b29851918368ea60a5079ec390f01e9addf9d721b012d27552a05a87b782aad0739e2f55f2db86dead99c" },
                { "vi", "959320f9be399c22d431636f8d2a4bfcf4f12b2acf3fcd0f5ed4d5b258c3d2dac90b01384975b1d6f31f3bd8a90491387b2a7a70f709cfabf976738ecf63d578" },
                { "xh", "0b77ee59fa8121c78995f1eb09abf639f3973dca0bbadc2de00ad5fe08e0c7c2c37fde492eb5632f44deccc80fda0f40bfa9476cdf44922e1721a6d48f3b3c0b" },
                { "zh-CN", "d6dc5bcafbd838e3df7f58d12e342600802830a5dfa84ab2e709175586f0c55554b483515fea27a02c949789ebfe75ee5f8fa3c665b06a3365b21d777b546d24" },
                { "zh-TW", "a5b239645f5a7b71a72448c4b7dd1b5127e3cdc18129be99a8dcb5cc570bc8f719976e232c4a737d2b9a8148a51ea0e8487f3db7c8bf3ff906be3aeca38d0f77" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b6/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "edcc40f9e5b7778338afeb498b055bcf4dff41b9b5ef7dd4c8da9495d31d207c221cd74481a585e30996aaf72bd9756ec2635990a0f0c5c3c77e7d1a2767d82d" },
                { "af", "1a96ac6be65fc47cc7b6f2478c742d8141e22ea0018ff20f4f242bf9bbc0dc0b4eabcb9c5e8a74559237c53bc72bd428f2cea2a8b60746aa38611c4bca5e23df" },
                { "an", "c3b6373a900e67fb5a1f458b221c6c4e9eb3ab93f027fd6d5f97cbd69f2aa2896a5f704ed7675c176ed688cbcf2a60ac4ca1ec56dfed092aa7651f1af4522575" },
                { "ar", "775c29068649ed8381e28482f499e9124dfbef0a94f90b542238da2a209d5f4fb5190743ecbdb8f70cb48cfbcb903e7a339c97ab33079059fe4f0ea52445d9ee" },
                { "ast", "e74674d0f3d4e916ecf1c687fb81c28f8fd6c4fbbdd859470356a7799a92d2378e9b525682997dfe95751323d2fdf5ebbcc4b6f7fc1b76713c543b6b32d80a02" },
                { "az", "ba04b8b9f6066a83d93b1dcec354bf066e773ee874d4a342916b804860459a6efe1784134ed81c2a694bae9c490119b80f3b9244fd3c404054449b82ed29d3ef" },
                { "be", "f7bc959dcca2005bf4871dcb93106a7dc838604b4ceec0dbda07758e90a1d2f6d90eec4648f5a3dadb9ecc52a55489205fd558d983352cd330e88d0b71f24b69" },
                { "bg", "b36b768e00f352309f1f7a8d1f5df4c5bd34d188743cd43edbccc8621dbae3b0244ae5991c38da4b7a1f83858c0a6be37f0044f342b597ffca3cbbc8e1bdce3d" },
                { "bn", "6c9196ecc9e4e73222bbb1311c96eb09729c7583e0456885e374c5a91d512a45814e574ef4f3275dec26d7ef9d791783593f37ea26137cffce69ef030400fe93" },
                { "br", "f401f5852a5bd2ae37837b1372d05fe1647177e7f845da22775a13c3f98c6e786ff1d9a440d199e7a1e676986a1fdec67e73e0c07f2c2f6d1ff3906bdc3e196f" },
                { "bs", "b3fd928e623b83eefbda6b2b47e23c7f1b7dd449390fc50cba8aa958f3fe15ac2e58b726b4d54c25c5b422141066fb7b3a586a9f39d7f4d1515b9b915fa99966" },
                { "ca", "3cb807eeea5ccc698067bf043fe29f6f3223227eb60cca045e691c64e947b5364d11cb8f98216c759ff801d956814de3610ab794b78a59eab31db513df5b9ae5" },
                { "cak", "25e3f1ea96fb6c7150b9d002fd2d15199b5003c52b6d6182afb9d12431bfdaba1ac9c665409505b621d222275eba6631a220fd33cd00e3a61fbbb67379f781b9" },
                { "cs", "0a581f9b0f2904e815e6dbd10f82a7ecd1262b03153c2ede7bbedc9eb239027418f92f598a4490b756d0160c1bc231676db8c26d50eca7ca569bcbe3aa751df6" },
                { "cy", "44b15b50ffd08cfe570cf097904356c29aec2ec041fe4697dc1bd71d326a3cdf9cc977a515dceb06fb8f7a319484ee0cd335d9eba7ea9a573dc349bf9ad652ef" },
                { "da", "00a95e0925e8f73fb78d295d2a888120fd1ff613d845590ebe6e7ef7653718c4b18cc009affdcc450ee2811f99bd7bf5a7802ef26d1c7a66c4e0339852bd9fc7" },
                { "de", "535a672212c0c877bcce677ab2c1017cde10065d133e7411016de26e192760922091abeca27d70fb1226a7002ac637b24697867af44022ec629a89633ec5b088" },
                { "dsb", "515d818248bb1629b95fbe070872acc89f3f686c4e56d335a7143a8af497dd08b0cb67a1eb730a77095792217605bd336d0df4a14673b7bce1734232214f492e" },
                { "el", "f040de3e19a6ffbd8e762a505973a908bf7492eef0fa0816c7226cc6f2386ec6987a8d3e4cb3fffa0ad43ca4f2c72ba4b3c185bd8a57b401470c191ca0e880fd" },
                { "en-CA", "dd916bd76df6da8a1c9b704af3b6adf02eaffce566b400acae1493ea978b215f2eb5a3ba84d1b6f62b4dd012ed5603b6f3738f762a1e99277d29847e4170893c" },
                { "en-GB", "6225181c29c2bb0c277c91f78058a3c75c863c8935fde982bc3771da0fa3d9db42db1a637ca1e8ac3f94409f0830c36fa0b9673cf0e1af33a0ba43ac53df444d" },
                { "en-US", "1ea4c7e955bed131bd3d792c266520ffe33d53526db3a7d33bfa63f7f1865d719e843b0173430238604d9366ebdbeb1f73f1d5531a14a167d3cd30c6b41a0044" },
                { "eo", "f67d3f119d22d540780e9f764fb15b4f50ed97fd132c1a3ef17f5a0e450f1b316abfb5d1b93eec4e17901952b17c33a5c4ad01b1dcee0fae09cc1c715772baf8" },
                { "es-AR", "948b7894ef6f95864f8186d1e25b93bb85739a3d6dc8ce082ca8dc0772e4a9ebee4ef2e8b64af65659ba9ca1130794317e740eeb9909ff33af7967b998fb9338" },
                { "es-CL", "60be1671c435f89626293b05948b41e8c5429cf40e228bf8f0c6be2f520fe682cecd6fe259539a2fd59200653bc0ab06a84b3ae166f2d42771713a5c3e8f9579" },
                { "es-ES", "98fae6f5f202c55fb761482b1eaf7af8126f160ce97ce6a2518ef347e62bd18f9fd6437d5b4f5a628e3193fd4da6048fc77f8123f13a8033c1d28edbf29b936a" },
                { "es-MX", "65b741fe13fe37b84c97574fe5110c94dc87f42db1e189278146c4ecb6ec344ae20ad5cba26b2929ab81f183fe5d6c1c35ee2872ae9ac186c07a22c33f02d2b9" },
                { "et", "990cf58df77975a4138836eac133f7c9490b780fa5dc27c1dc0c20a7ce1835ccea280351f764cb6921aaa0e14040276798148fd3e13ccee83ad0c9cd7fc79e53" },
                { "eu", "d4d34ecccfba84f469d2b1fc3d7ec621fe6c78179b7792e4a1345b2d262a98f1aed4cd3ce9c11c81c76ee3e10aa1ac531279020ba04e59ffebe6bafa8c7607b2" },
                { "fa", "c034a84548d42dcf5340fce0aef2094f86470cceb02ee20b83c5060d2496808109a7196e1cfbfa9ae927b5e55a5ad0b0f18aeb7223a631f3094f137edb29ba7e" },
                { "ff", "41ee4f6c28b16dc281018ba54c90fa725b9bec5e827ef7eaf35c60b66881a9a3b472b72ed4a515328488a4f048acccb4a7b83872502af83fe7fc92edf188c1ad" },
                { "fi", "2d274694a861ca4044662f7f0259318eea8a7c4d682646f0cf0345da96b8da5c6e8542168b5ae7044bc24b57305bf2143cb715bdaea2232f7721a7e7b89d15aa" },
                { "fr", "8b82fb547a61965724780a21b4060ba7ac4ef06bf1fb31c8eb2a2576b733d8011e8cce0744d0d50b8b5f430beeb0f6c8b41039b67b879ff660053ec71ab63fec" },
                { "fur", "019c3ff5aee53f3fc7e831c526b009a9f31f0f9cb507e3bdd887cb001bed3fcdf371cc0404421c71b8fa1d804c63b294aced7ece75e49468ead971fe3ca3eb60" },
                { "fy-NL", "8daaeed749c828850c382c462d5d32443f33474dd85b80bb0778c0c9c3320ec78d42d8355d8c6ae7394201f9887e1612d3093bd53593e8932cc29988dde1f770" },
                { "ga-IE", "dc29c38aa84cab3106c49a905a19e3633f50a9fa0d97b01ffb7941476344c26b680407f12ba9c1175aca98bf322410664bd0bd8ccf5b3bba3c51c8195c74660e" },
                { "gd", "c8a9520869db811bdc332c971d76416b8f9615e293ee67e79536428abfc4d79893c22874f30cb4f0ad391e35efc781baf3eda7c09f0a5eca4fc4cd016732a15c" },
                { "gl", "0c00f3c4eb627fa0d1874ec02159542bda1246e54e1a127982638bfac8e80ff6cc90d41c731fc8f2b4336240d2e4a391cef4f1cb18b5d9ea5e82f14649061bee" },
                { "gn", "323eb0e158d51bb084e32aabb5376ce300cd80201b8097bccfd4a0dd9796dd284407c177d5bf07cdc846f00d0581c0da2ccd2467d14f5a6c51c5fa36744c4446" },
                { "gu-IN", "490e704bba69fbd04d75caec85864d5378f8c4fa759cb8c9cd9c028e1bd19fdc82f9f613f429c19206a2309df8dd2ab2a9eb263d9ede202010c60cea272ec798" },
                { "he", "e60f7773d5297ebc28e4f3511c344edfa642d9c18e10ffe8a4f725b1ddcc81281f913df971dc2a2d9b673d4f95e8c522da016c7e1bcf7d6cc155f6bf9d145340" },
                { "hi-IN", "0eeb9debbb8731f2cfecd99d4b5bd5bdbc168fe445f75ac3367dc42101a97e7b45a059dfabe8d16a3778f7eba6c61696f0d2e68a301f6bf993afd9321f24cf91" },
                { "hr", "5c7f6acfeabc5ffa2005222f8dc31d9033e99f57b1bb8f46e774760ac2e82823e9eb14d68f251554458c31d2b8a4b901ebebd18cb7e91e05153068af03c8209d" },
                { "hsb", "45e8ea7c5fc77c18dbe142006771ec09e11a5a120ce13d5bf2598f05e0334cc801356d2948d603d265efa738722260cdb536207a3c1036af99ca3dd737b35d6a" },
                { "hu", "5826d6c55a3f90453dca96a07cadc599139ef4cbab9178cbffb24b3fbbb31d74ead88b944c8d7e042036a613234201c22ac53622b6228b8cbe0e2fa9d106d312" },
                { "hy-AM", "3116f46bf8fb0e3de6752ce4fddab87dcec62fad9c7e3461a29cbaf60e0f80d18df07da00425556b54a02071eb2b0bf9f81bc12b9bc261579358c7bbcd710bab" },
                { "ia", "b7453f18aa8348c8dfb152483f3430f0a202020c9803c9d92d305c715e650ed059cf1ebb44434e72930b98e88b009c9f0170f229e5c0c0223e3ff7a1fb27e442" },
                { "id", "26025030a7dc7cfb1aa4ad94b81abc09e1f43d989e58e217c3f9d8964b124a83cd79ced049ff2973f09568047551e83d6cf6346128f8bc6748f4919ab427c69f" },
                { "is", "f2feefbf72a690c99921bbe393d47744ef082bfbaad67e17c76b606dfec2692e421baa832d5221322e58dd74ad968b69675665901144cbec94ed1229f342d8b3" },
                { "it", "e2ec54d0ad5c9587125dd6b1a2d6461f34ce054998ac13aebeb818b54b367fbbb706917ed4581d0ae71e329d23e628234c4de93aac0733f75cced655b511653a" },
                { "ja", "e121c5eae3c9022384c3499721b6bc4f44a921755dd0b09f9f5dab6a77dcffc059b2b9a39217a348fbd7a766d50cd6c70f985df348426d68970307af36d2471e" },
                { "ka", "5b11c5136b40600e6efdb5d6c0e66cb6551974edfabaace41cac177360475b1de7743abd4c343172617a3dff2e932c1c5f0ffcf826c369ffec89f0cd60fc0e52" },
                { "kab", "1ef107b0bfdc234f10cb5f6fc2a64c3041944f713f520e5daf1e068b64cb1bf6846084aac7fae0b06872827224416c37e4d0a46fcf71393dfbb4236685b0bbea" },
                { "kk", "24566a970969db58e4571e1267aa98cbd45ca454a77683c7caa662dacd6b56e5ad06126214af596f1b6e6d1dfd4b427a542c3928d78f02bf5213692769e77423" },
                { "km", "6e9b4b7b0fd7b7799526a272e126c51f8b045325f7316717188225300a3679ec621a5a91d034518f4d716361b9294b099fc43f8389e5f36ff0f04a02a23f7aba" },
                { "kn", "45fe0b07a6dce19089a3aeab9cb658448e309fa954239f3b478bab0106657e66ffa58b848e5d948466b29c47802f5169d0c8f1ad78b23fbd71621006796465f4" },
                { "ko", "49e93d80b024680c9b682811928f033018ffa78a4ddbed1179261ee4add3071b66281f017359d7baa389e79a67e6d226d028a82d80bf65ae7c449aa6556b8847" },
                { "lij", "5221e06c196c232f581579c7ab45a1628b60b7fdb7c47611fe0d43a8364d955752972c653d0ae2c5cb9c3cf6cd35d579a554fca6913eda1a94b4d96a5794d026" },
                { "lt", "833070d07858d24ed0cd9426f1e4741a143659dd50a04fd730317495d6d34d9310a5f3fa64850bbf692c42414e03e7a151bc97f5a2c0f5de1128533b800a4c5c" },
                { "lv", "3001448ca841c92feeabec8811c524143baa559d0d237dae8522c7a3f822f3e3075d117356dbf313b5f9d70aa421a63a8709d3c7c8af7430fc66a095e6a58b6e" },
                { "mk", "839e72f19bd737be6c7fe26da4609b36ad75c59afc784e6dc77548cc529dc0825a190d1265f0786ce5a0189dfa1ad4f239f99a55259fde18561ea6e1b05bd367" },
                { "mr", "d303fb035431e2717d0ba17bea152aa1e075059fcb7159e6808670f0d506dcae9efc110bbb4b8360bdcff48fef383337bf89d8bd6c5f611aac0fa90e140b8d6d" },
                { "ms", "b8c34b40d4162f2ad27a73493d0b605d230b56abc6b37a086b749066be43ec45eec10566895e0347cc2e098fc729e7430c84c8d3d8721ce81a17a54251394974" },
                { "my", "b936842dd6e71f8447fd86088d8b905c1ff0d0180c5e96bfd80f08ab64ad007175ebdaeecf1d3f31a54b73ce457f0924babbea13b91107b99fe1da5afb023827" },
                { "nb-NO", "8c8eeb360685cb0e03d989f003bd6057915fa7e06ff35b6cd95c1262163049de29610dac0be943cb5f4789d01bb6aca28f7c81d12f01bdc777d8a317955acbc5" },
                { "ne-NP", "98ab87133b4cb6098b163a57469eb8f6b0324eeec96cd3e6e3ca5a4d9d22703b700a855b87ee9a4bfbefc980caf2bfe469a4407a94376c354492cf4251c4635e" },
                { "nl", "86c6d65c9a17bf016fabc34a9224893a5510cecbdc2db1213312aea4ac475b1e3e3d1c0d1fc140266361dd63a979a3d512170128eb80f0b3adbc1d7a5eac6c9b" },
                { "nn-NO", "da44fc7a677136c788717837ef8e26fede4d1b403d7c652954541cd66d7a5fd42d3283ec5f6a28f0663e9ef40134360fd707330fc40d0ef5b6bd35b566dc6d3b" },
                { "oc", "33e6907bb97b00a9bea7c86b26843eb45f845053b813b61ae9e7470c24fb5957d6af13f4f2d54b10c3fb5f5ef402fe4b777080e57f48010cbdf2d0e9aad44d99" },
                { "pa-IN", "01a899cc3a8f28c0d7b79e2a395a1a0b78ffc68901ff81f892cdb2b26cae0a943cc0355be84f191e54d24c712c271bb61e9d539befc8164c64ee2305bd8e4a7d" },
                { "pl", "7d7e55fb158ed2847ea4be865b4a9d4afc55b0ca5e0d052b94780b6be154400459043247ccffad527e914dede79b297584ae48afc89c4ca721386c100dfb57d3" },
                { "pt-BR", "805ad7163c4d71147f69420b03aa66e8ddbf788fbd824ad5c67f5425de26f53b0df730821380b85e44b6d5411b9d9e54b8a70291af1563619e2fa7db3ec883b5" },
                { "pt-PT", "7abe7c0a378817a8a8e536f039608f97fb962366aeffbcbaa768944c1770921e36b488140904e0a3e72f1fd569dbd6244c00fdb0f473c91f8c800587e813866e" },
                { "rm", "c6b12c5dc9b8f565a8855ef08a4cd95b847880e4db429a588800776deb87ebe17f55ecffaa864b41a9f831cf418b3cf7e7303e6327d0d107ba49bd1f9c8aee1a" },
                { "ro", "6e778e25c965e22a8847be8d80e548ad87ea5b3d29c335b47ceb3d72b7fe350bb09be572982f0e858410ebb7156a4f4026f71ff2078e63a77e4ff706c86b7174" },
                { "ru", "98996aa648ba09da9fc04494e33997e87ada509aacd9e2aefb19465abe4eb015e33c67feed2decf8910f125151b8927de7acb707c644facb06fd8e6097d15bea" },
                { "sc", "e38b7c9259ceaf1a430756a5572563d92ff45958e7c21a5272630b3dbf1dfb5efdad982afd58dd0e9528cfb20eb0331af267f4e31ee499b312d89b940d9d9842" },
                { "sco", "04feea3ea65181fa30f4d67f99400146abcb2f20e677d5e340559312cca41fc753ac3215f5c0bcb81b52a52f512054858ae3a243d27684e39e5654a1dda4b1b0" },
                { "si", "57e20d029005e2e1565ea0fadfdaa8347abe7715fa978718c1633b0df062362c18239c79a95f4683a115f5b9c55ab35f0b3e775e7db3edda1f1de27a32fc66b9" },
                { "sk", "2e941692391eae64fa88334e2392bb3641f91bba745e7b93f33890fcd774d713c3b828562b03519074b0e8112c4063a3174bb4008e9b42588f591db71e1a57a9" },
                { "sl", "16d3b25069e64ea7073c1b1269c0bf81a2091e58c86ecbe67dc62d9eb2bac807e18a0c57930f6a066612b3537fede81c226eb857058be31769c2e345d0e7a003" },
                { "son", "96d34883dc7f0bf59f0c21652b70e8254d64bc77bca0ccef6ed7725b900970060d75c16a857ef36c3367d147808fde877defbab65fe871e6e91f464f49bc2686" },
                { "sq", "671ecea88037ca6533a134dce1829351e9cd734eb06f78660ea5630344994b35072f0a542da926e01f6f26b73ae6445e162a3d58ff28ddc2ed2e3b8df4e3a5ca" },
                { "sr", "18d24a0e694465b5a3c627127ad84993b0627c3e89deb46ab69deeccbaae74c086ef24c7b900e6982d7cdc94caf015af7f9ac245cd82729d0dca776f6fda76d5" },
                { "sv-SE", "59b3979a322485977688d6796c989c4bd9ed64bf971254b54a9a36ebda74da21348b293c3ae29c0fbf8854666781f31ed314cebd04f6be5d8b6d82de763ae1cd" },
                { "szl", "14f8d6aa0ea069c0ec402f2dd8959e73d802edf3eb66b4f39693a88db581fdbecbb138bd6bf905e89dc210ffb918261c3197e7a272a2b48844bb8da7ef324c61" },
                { "ta", "1eee09871550359577b84baa7b848836e5818fcb6124bd21705008324829b29c7cffcf12f6206fb2abfb7fd646eb0db9e3c1e23359f3b22b9585b06769854b07" },
                { "te", "3fca3d4b0b6ba7d5bd8c8e31007de11080501cc9281db160bcf9dbb00eeae59e1173ae4b439659e45f4f8041fdd98c210ef0ef5a99e7e38c0ebfd715ef29da36" },
                { "th", "374dd01152482a881a7aea4f1e6a2864f8d74304d25f75e18756493b8c47d358e35c3cfb642b833e10a17cdd89e1995e99533c03ecb546da4794d80054cc191e" },
                { "tl", "d1a037ce510dde418577cb5a119cd5ee6dcbb3478c6347ceaebbaa30c5aff98d7cccf4055f6fbeb58b2873685dc4a5bbb9e44bfbb19d5da543c482623065a537" },
                { "tr", "0a0339c37d27412bff18d64d95d93421fb6f687274c490f96178e6df87311443e69dc86b1eb7c3b72518d645bef950a0ec9990bbd09fcb0bf81875791256a61f" },
                { "trs", "aee7942dd09f5bff44e911643a12e6aad59ca356ee590dd4ab290c8636296bee841bc02a4944b9c9353a7c13e8632cf1265aa60c591f1b3a07f5b700e84ed4c8" },
                { "uk", "509c58c0d1056780bc9796b71dd901d78e242376edc6e229922ad03ac17981d0b98488fca486a89edfcd85212031f1fa325f6c9257441f5747225048dd021c78" },
                { "ur", "24c88acd8891ecba1850ac78135d8747b93bb3f83b9350eb9bdbb2a0dc642c0b023b6176c126f9602414bf2eb728806ce454653c7b8fca805ae72661c5535fc1" },
                { "uz", "79a2d54b811e4adcba0bf75287b5e425a543a2eacb8fd37fe331cd83b9260517e37bc2da68361b87ae005a6379bef79cb42aac5794d401582729618b5c789ab4" },
                { "vi", "aaab79e576a4abaf80e50d9d9a47ab2c5004a5c6e1a07e9eb8ac8668da84502256dba8a748ead7d9c5c16b9fc49569fc39d85dff2805359646f68a406fcd27ef" },
                { "xh", "3d4cbf8496a064893a76e96efa201ecbe48d2e0edb053c3937b72e8319c64cfcd42d46fa07ca3f47cec52b0df7b1eb27b2cc68193e14f4929cee8a2b98d332f4" },
                { "zh-CN", "e57c738579057ec60de14ad02e464fb6924fc076564738295b543f51ed68aadbba1384b613e45cbe52ffc4ebced819d8e2ea3e0fcc7130e0a38903c04cb867cb" },
                { "zh-TW", "478878bb011f1b602b246872de3048474d761772fc3cd1797b8135d4f070678ea8b3addebe31923404fdb641aa4dc48fc57bd5f52719151b0a2c6d989b9fe011" }
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
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
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
                return versions[versions.Count - 1].full();
            }
            else
                return null;
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
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
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
            return sums.ToArray();
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
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
