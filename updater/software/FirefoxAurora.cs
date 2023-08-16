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
        private const string currentVersion = "117.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "3981056b2bfe255be357c9849f98fa0280410fc6147534ee5a67095cfba1103708cf8f4865d0dd14a271a416b4628054ed2fd87f1a80c0a1f2da4328c4a1c419" },
                { "af", "0cac2ea471cc704266ea6bcd02a3dfa382b13e3105b773e6aa41dbfa3e9ede7fb9fbea6194d11bc0101ea6d28ede0c3445c27545d4f4a70208a2e4e5bebd65ba" },
                { "an", "142bdcccabdc61605b99e0d5b70b5c1f8ac95886f11b371f89dbc7cd42857f2712f0e0c06aeefac9d1de967ad80f68930e8dec27056cb587282c4a5fc740414b" },
                { "ar", "94821ba355eff8fb509900c78b4eb5d60cf9e7326491e9e69aa26565685193e63783e9b90cbf158d0bd68c6dc6ef5d2159ad942b864e098a4b42e2d03293a7a1" },
                { "ast", "23610d143f8a651ddc236d19d57865e45853ddb642d186f7879c4f0d0721a96d7ffffdbf4a5f61a6ed825b998764cf13e324a912b2f5a46270b27b153ac31897" },
                { "az", "933fda9597e4a8561528093ffc3dd7375f5a2847a898995b4c8366b3c5c80b3f37ff1390d261caedafe56495983abfcf759ce4cc1dde080204b3e2d49e9e3577" },
                { "be", "ba3fd0c896215982c1a538045dc70321061fe58bebf1b85133b3640f89d947c8da4089744a49230aa9342e31e1a045016b20b68fee071e2b44d01d3d3267382a" },
                { "bg", "56cadb5bb9a527683f45b8270bbf9ea943dbe9e558480c443ae2e01df3d8e66906a685fb91454fd77686e8f3a2958703cc1a57f72c9a6518324b1067aedd1382" },
                { "bn", "b0b12bb1323b0c8496fef1280fcb2b3101f21447590897f70c711a0048c4c423490990b446198ec9bbb12dda9081659ea76d80d7a4a88c8370b5e48cb8b0f2fc" },
                { "br", "0ad23390bf63d033bc711b02074b04518ff19e5b34527521b9209ef95318a3663edf8c4e91efe38db73747a5303e7bdeb5249f223c742c1aedc07ddc0ff8cb8a" },
                { "bs", "601990be8436ea47a69182b6017153dc4c28e5bc55f54b2d74e1e9a58a23a7d1be32e95e5b4817c6d5dbdce4b53b2710af4700623405f8fb0edd0b8e266b5eeb" },
                { "ca", "31cd039e9e873a0c0026758676976caf5aace18772700a3cdfa285f86366a725844a2e4ff53554497e49a38313d1d37f16e172a16c7b5b7fd81cc88fd81b865c" },
                { "cak", "e2423123174787e48fab35fca08fa2ebe27c546d7b1d948ea4ca4aaccf6593d2e531d8dc91aed73f46ad6633baaf9b2c329f415038457fa0dcc24fc28bf5022f" },
                { "cs", "fd5ff42d01fac4d345e5ef4702447de9dbd85b58e3acdb40d6453977cca671428623f30002a40aa835bde5a3aa76fbefb81a12dc2997b2e7e467dcf395a43803" },
                { "cy", "103d0092cc1214ee38b27173ac367267081ca652ffcf3cf0df4f18e4ccefb4325e41b3d12becd5a09cc89f01d692c83a84bfa9f49cdeccc17712b101071b117e" },
                { "da", "5c6667762124eea298890e2212a7c743b62bbe2d978e1f6ef86f70f63ccb3446c1f78ca3e2880b0a25ce2674a87fec26cae4e61751530399bde910af7c0a4610" },
                { "de", "294d67a04ddfdcdc1c4cf56caa917d57220e80213bd893f8bc06b739bd331f749ae1e6341b6e5a6fec92e3191f40495c954872b3e1f689a540d4f8ab8e276adb" },
                { "dsb", "47cd27a4b66c0b58e12cbf8e8e7a824672ff2baed3ec0be30882f182b3c30095f580256131f5d7875777f53cf9029d5f8fdaac07324e7b0523fff77a6139f93c" },
                { "el", "3cb3bd687bd07ce7d4db9a440fcbed554aa092dfe5b7069420776f6ae4628e406d73ef153ed57ede1e45d29f02451ac89515776011def99b12ccc6683dd4d59f" },
                { "en-CA", "6de686175db28223b3e7cbf65114a671cf674d94d52f0b64b136d4fa2645c35ce011f09019b804c80db49cbd62dbe4f79c4c779ece1ffac6082de3f0ede0dddb" },
                { "en-GB", "6e53f1a76a0cce0de9113c918e58dbb8adacd7ae2fcd00f21ebbc8f663c5bb2c343767e8cc96a6f2afd7436fb53c5d6929faa500f2f8d0aa3c3fae59fa86de62" },
                { "en-US", "af44c82153821d72c07f7632f44518089eb645bfdb011d3de1eb1dde040a8b90f2413b01aa869c74591a5dbdf8084b63b6e733adda2e5c4ff299ba50628009cd" },
                { "eo", "8c3292253d1cc88f66ab20429a21dbe12f91f74a037348fecfbf8b1586914dcf5ee8c6c823185056d88d4a0e045319963c636c86209d513b74274eb95e3e4665" },
                { "es-AR", "038f4a8c35729bf73174b2ae5605772d39c28030cd75f1ff5655360ac7256e105a2fff0672dfa46fbfcb006f03b44cd395d42cebfa6f86341b3df3b4b9452d68" },
                { "es-CL", "c471d3fd0cc641ebe586c9615f603c811a0528da736637037c2cfb1a97d10766be4ff8bf29376628855c060bb2dcc7b4d2c315ee0329b6621d6468f53516784b" },
                { "es-ES", "5c324b7e1ac39b39bb9e20fb558bd34bee3664b2bbba71d4d03cbdc8e02d444ac6d1e9d67961e2e84da9e47eed36c9d6dcbe7d166a10f2a900d34143d0c7ccf5" },
                { "es-MX", "b632200cbaeff12d02e6e0c0ddcfbe94eaf311a7fbe2ca7bdf31fe5f40a0831e2e1415642cfa02076337eabb1aa7510dcab711dd7afd6947d8e9dac058d7fde1" },
                { "et", "a7583a8454b247dd031e55899d8250bf56f5043f47d34529eed83d009e3057f6f38cb2c9debd0784f99b81eb97174f5302efbcfce83d790e47d56eaac0fb7e9b" },
                { "eu", "15581ab7bf45752486664a6410aa4337a3990982f4bb30dfd75556bfbf117b4732f1e8e1ac6ef1940ad7c0774dc57d976d04073dcf8848a6e79e2c592be327e4" },
                { "fa", "af2bfdd72849f006795be4b7c42fd5594fe3c5cb85f4b9e91bcf9d93ff9a3616d63c3047f6d0df85bd5da7b00ccb50cfd2fa9de58d35bacca46565de7ba4427e" },
                { "ff", "a4adbabd22401c831d77971f945a0f14f33475acd773e5357fd8146df13b9db0f911e7005d0f6d0883f8c6b4656fd28dd63c654a4058ee4908fb2deee8ffd778" },
                { "fi", "30454435c094b1d5dc925a1c1bebe234fa6925af406533daf6360334108bd3674f1e49bd4b231bc7d9c3418ecd99b4de55145aa88132e32e39270bb1a60ac604" },
                { "fr", "0b4f955e8456bf05bdb6cb57a4d81821699481f840dd89bd396c4819fc707a41849fb3899868ad924febe11f1e017d82d6e216b15b210c217746ac4976e49092" },
                { "fur", "3fc04d551d9f1c220a565e5d6c6f48816cb6b3786ba961d67ad4f5cba43c6b19fbe7268cbc968f27f869b82539d73843de14b14f59745ff98b1da74cc9e7abcc" },
                { "fy-NL", "9ed7614cbbf4c5373c438b3e194a2444d8a50477834b58e3d28a6e9f961ca4f5a4faa96519cf240fda1ee6154e3d9699f691ea611dba6e212e9af552663e90a7" },
                { "ga-IE", "a598058215d80dba410bfb45a3c040a39ad69b814209e2554f62c343d0f7fc00c2c4175c06cdf49e232888665336d0dbaca14c224044d85affb74863a020e9d9" },
                { "gd", "3a5cb730c40cb6bbbbf86299078714113e797f8e16b83b19df07d3450ad873ae0dcbaf807b91bf3431ed87eaaafc3af41fb16bf140af39ef53636929229ac04b" },
                { "gl", "f0af5efe1484593b657b94eed13c9a1e923bd8b9cba91afb32e2656a28792b96cceade582ee189c96b141306bcb8507e80729a9095f4bbda39984a658c461448" },
                { "gn", "3fe8c3b133d97bdf9753c7ea916aec2771008b342c2e1bdc870f69303e1951b0faa960a1ab0329f1fb65046fe26190e195ab7608304d3bc16346b757b88cd139" },
                { "gu-IN", "fa797f30e15614ad5147d86cda1ee59e5df2d5343d22ceca994aedc44baa06c8e9ad00253777cf412c2539ab9a809b7f97d97c8542a99cf4ea1b9b747e20f103" },
                { "he", "6e8945634c0dc065368e946c60002c3299ab97d52845cab24da353ff0d8b78a35f07e26d46308556e6469315765f1636e255afc8f4989e3d3c8df458803240dd" },
                { "hi-IN", "929d71d0168a142b791a6fb96b8ff768ef6eb542ad48e5b3b87325fc4f9c3f19ed761678314bce997c8ccddd1b345fb89be19180f6facfe8e66d9ea3667e6d80" },
                { "hr", "38a8e18ad7bc1b81c0336477bee2bd7c41bdfec878409d98d0661bdb0751e5c5ec069e186b939e844b99cb28823f37faafe6c45d664b7578fa24e28847d24d62" },
                { "hsb", "90e87c304e1f311770e94b67e16edf12d746b51f5104dd2748517ad847cd1e447b9d4548292b0bbb4db428f1a86bd3544e5f1ad650df848e8fb98e2d6b481b63" },
                { "hu", "9837f475a159d63fe9f9dccc471129e2a01ccb455de7e7a62e66f69e7a3aa186a531955f9b3668c0cef6c45bfd0068b94f896651701cf5a05f3c6ae327c0fd86" },
                { "hy-AM", "99c7606c2e3a089cef4d143917caf33d643945fdcb3763fbc82cbce0e096c08cc992187ec19a7dabefcd5045ba5fb1a24d872ceab6e7ad9f61e738f44f5fab5e" },
                { "ia", "9e4e6d640a3cf120c1195b1b6c4dca1a67b53672cfcfd6c8179d92e1ad8a1e47aaa3b8f4efb422a584d56d1edcd7bd071213a1b1cd9caea490e8d0df75f05df0" },
                { "id", "52845807829725b35578b7edfc898928fb4d240fd23c485e8b91f1a9aa838242090dcdb6206b239ed7df2ce0ffedad2948d9dd1cec801405e160964f6fe359d4" },
                { "is", "7f481983d45d65c93c0fc941d7b231f11ce9844dc47f251a9261f283cd657dfd2fa28d3eb011b988791a4bdafb1fc7961c8758044a55f017704259d56fa94f17" },
                { "it", "9bac4abd1f129f6ca00f57aac3364b5498b48bcec9f8356e8662c0f31df790c7ce5616e5ae98e910a816cdbc778fcdc7127df827681d3406d7021c3a3cd7a431" },
                { "ja", "63db29da88760ca952414738cf55614d8f1cdde16f22a84ad5490fbf8a4d2fe3249b162d28ab4d44ef5f513336b0caa0fd35fb53312de4330b172361a5fff1d0" },
                { "ka", "36a06dbce0b25b390ae989d08edff3440b81beb73f3879ceefb9bde17f79a20ff37f3e3226e02ec8be9e7b5746ce75e1ff5b050144cda2bea1b3981f3022c8f2" },
                { "kab", "fef68391d7c56bb826ac68d8fac3038255e2e81a5ea5f6935411167f3b54cbe5232311fab29c0cfd7dae3e9f37396d6d2bd1cbd96ab2f40b90999685411b8b01" },
                { "kk", "9cb22a5ca4b700d24a60ba9e2f7a4d092072961297ff61ff423bd8677bfc530de3d688085df083cae93a1496832f383dda3de5e594cb64ef5556c667a085cb05" },
                { "km", "2b7051e0f6557bb6d1c159dcdfe1d9362b40b55601dda8b8c066922737c30b9ad966c9fe2c9cbe29efe22e65a0cd1fd01f7575634f5fd243bbe0486042a94c6e" },
                { "kn", "9c25afd3145a52bc515e396e6287dfd1fbbbe31c4e14df3440ebc8768d6bb1f74c6afbc0ceba3dc4176d5df7498023c06bc9a608d0c187bf591c27087a863de6" },
                { "ko", "7e0381a91912892a8a4bae7a3e10364534c2733edf11afefcc83fc62fc123b374e7b9d1520ea84862696a0f54d85728708942bbc73ef88fffc1102877cb566df" },
                { "lij", "821ff6d66c3ba6bb5726778ab7d80dae5c4ed104b523cc3850d25a4b6d854f06f334824522e6c389cc4ea946eaf87ca90c6cc8b31f82e04a56a1dd539e9af029" },
                { "lt", "dd0eeca4b5921a4b106681920db0d38d89127fb18af1521db83b2ace2e0c64ae6dc258c31bbbdd697c3f3e0e00271ed200b5d949bed1984d174493414e9a5cf3" },
                { "lv", "0d3c7edb764d208d51144a88051205ff54146b6c95136f4204933161c46b4e89be4275aafdcf5b42ab816fa66c126f032428e6add7f7480eb04a3638e75c302b" },
                { "mk", "71fed671477bd421d7373d22dd33c34f252bc3ac2859bf38ffbafcc4fd7edeb9222f8ed074d8de7cab0e90e9ff89935c05613a5e8d90469edd8500174fe27ec4" },
                { "mr", "978e2b59287eb331b4b5c16c3ae64165d38288354478e26a429c8db40a41b1caeda6f3109c8ef5195be5479a7d202e46b7eef03acf35f4ce87529b0c12b8cf72" },
                { "ms", "3bc8f10c8f85a8a900ad2f555463885a309c611a83d13131e97cb7e50aa56b4ba5fceec287b487bd3e73347edbf6b06c5a35b45fb2c24a17cfaa4915a2d5fc4e" },
                { "my", "985a682133ee2b7ff92210497baaa152202328906510318099106737e097a5f6d4fdfe152dd2fb7d88da3a99498603dfe75863d640e1dffff679c21467e20932" },
                { "nb-NO", "1e09d6f920c0552990af3b877c6bce542b569ecfa6ce184c6da2bd1e59fc82ed22da75526cdb6b4f58064a5e94762c0316ae5e92280827bb4cddf9d323a00b6f" },
                { "ne-NP", "74de888e5ac03bab6fef9866565bd7a4feed4f666d9e1794a45a4f3144469469488a76f36c0b9e52ae40f18c73347df0395b533d2893ce54630fbcf0f013a9b6" },
                { "nl", "2018a75d0df11cad9465753f29d9b567a16a6f54de54be9c94cee0b90b87a0f7e8c23c3f752a80234a0df2af06fb93c7033834a8448286813d1f89136d13431b" },
                { "nn-NO", "f6eb86a78abfd2992452156b49c1e1f9d0464c74ed6c711b7ed99cde67bf030fd5b315c8159466a2470eb9dde707ec14bf47fc2c0b5f1d236da4bdb4462410e0" },
                { "oc", "f88ff12291f5bd7f324addc90ca93a8898e90d96085c6405ca2ee5f3fd6b0c8c667b309a71c3a03f3b14fd598ce2f0fd809cd2139b521c8ec639ef8dd74345ff" },
                { "pa-IN", "1bc09c498233ec250ff16dcff0fa4101c6fe5c33ff3424d80eb3b69ede66a6bf4a8a115063a5534a2f88533f96ba7a1157a8445369454716d339bf3343d73a88" },
                { "pl", "ed3f1d73e2377c591ea19bb9d2105c816915cdc2b1b39bcb6d2ce3214d92b84c25768e235004b68fe9d41287179a0d9f9727f99b8a8e45caf08f4b34cd14b37a" },
                { "pt-BR", "ea55b3c78c6c290ce9225f62dddbc309cdff487d8eddb434064e1598f27d56d92a06df4808360043c216e67bdb52ec20dbeda3a5de6553efa148c7556d0e4f93" },
                { "pt-PT", "bd779e96f42d14c53ba10b77e0892f9fb74b2d8b4f3df6501f6c4a693bfad303e846783b720a985e124c0cc43c45f307bc4ba0f0062fccfdbf527d0b259e6687" },
                { "rm", "5e4abb1fb496a0d10cc2b81984365088f851e35555c4257590af7c780cce1bfeb03db8c1e50bffcf49a42bd6d43cda8690274569d71f3e05b17c7d41991f4a36" },
                { "ro", "4239ea4682976e040e016b8cbde8c57bf8b38157a4c58c64a3209fb87e8200320dfdcb97af6ad2d14cfcdc40a8f7ce716117626d5f0292239006d9ddbf5acff5" },
                { "ru", "6fe08d34a274f7b36cba8a95ac5412b119f4e2a0fddb80e8710dc5bdf40dac0fa62528a53395ae6c960fc27d684769fb4555a3df7c1f60c81329af5624245793" },
                { "sc", "ec07ceca2078ec220a1e3fbd2d2b3c4ce83df21a279e5440e8c4c5cecca38cc773ef3968de986f8ebbd2d6490e16d7f99240d6f8e64efca48328b6db1ec1fb6e" },
                { "sco", "fe9d42b9db4151215a8281c45cf21bccfde750e0dabb44d8fd4489e42c4eb73585121d7aa4d62284a058f74ee129664e183fe21be389982a97c55f97850135ed" },
                { "si", "7ac2f959f7b20bc5a06ce77682df918f37b1f971a7aca9853ac0513dc3d1626b2a7e19f3248daf02d4dc101544fb26ba242f85aa04d8b8740ba91bb6831fed93" },
                { "sk", "b9f626c6c6208e1edfc31efd1c7df8300ccdc6b77fd2bc03f3716dbb4b5347a44ceeac77c58174c4f74651f0bc67cca0041194ba542129793ce06523f830c4f0" },
                { "sl", "1d8390bb96337378177b1488360b8e42299527529a11f07d55b1f7b9f76d2a912dd61c08eff381048638192e00b3064bc0507a5488e3898ff74ac9d63d25cba3" },
                { "son", "5704fd52d0b72eb44d0b464ae7a9a47fbb641fac32c0936163b8be4ee2b679bcf48db68f008c4b76e872ed6e7d5162658cb8c652a56a8853d429b8b8e3d56045" },
                { "sq", "2a22db7a9838f7177a71d66bee66bc1f26c67bf305c0bbdb6fa24e1cc73633a8558486da46c4a4685e9d9dbdb91cfdfb79bc4cc1706a35a285865e84e0f777d7" },
                { "sr", "5308f2fac333777cdae75469dfbd406a1ce67d76a1400c62fab81dad22f626af88d5d0b7204e5dd6c33c874b321e9a524781e70772434d67df670dd6ed4b7f29" },
                { "sv-SE", "15b5f172bd207dcec60a9cf009ff53ae77daebcdf49459e289f5dcd0c48b86fea08d52b1bb1a4d5f01c7dd054804f3ba524320f41d64111484cef059a77471ce" },
                { "szl", "8c0096624cca064af49b3dcc77bcfb89ed9629535a377401e663c7feb0a5ebf5ed54ad9bf312851d8b0ff8945375cf755a1c8e6131351d56f53624c359e4c35c" },
                { "ta", "b4c39c1757d6d84e2e7012ea0dded110c32d674ecc33c09cfda7f943f3a78cf597c3d1b3ad6fac7cc76c3607fc95ee35d53c6849e90ba3e256d48297faf46b01" },
                { "te", "30ddbd91ee38874abea981d46c0167c8d4c171c9184b557f4077c04893cdbe6509f14b8e8f455aa864a84759e5ba295f638cdde56d1f1d51023f8e98f330a441" },
                { "tg", "df9410889eaa6222a67af23c4a7400264885aac34ac27ec52ab7a983396c783496c4fb1b265095f20a49249741f3dba86dd296af4ffa99f0b884ee4dada175b9" },
                { "th", "4bae729299927b6ff5a35a7b6d8ca66df366fde639718e7dd0dff9ed11348567c8bab900bd66d4bd7ec4ed4755efe4a33ee9b2a2ad4b877e6cdbb65560c94aa7" },
                { "tl", "6a288e6362a58f47ea2bcd22a4cdedd721857f7b23cf02bfaa2ee342136f8928a4d0f950947f4e69e9eff217a576b28eb823bad6c0cb8986133a96458131a1ce" },
                { "tr", "e40353519fa82a24b11ec63f37125b87d1393b08b8057283fc5a151410745a58adde6011f55730e4b67a4258b309520aceedfa962f8aac932b3ec1154c4cf6ee" },
                { "trs", "874201207980768b98c24fb24052a8a3dd0f77e974ea3dee923b61bc385846e707a1db78bb16ed091eff569906b6d5be63869119d2b666d1b2ff977a64b4aea5" },
                { "uk", "b913927102f49a42b08abf7e09c965b371798cdff36a5d993292badaa7bb01a4bfa74dcdd998af4a57c8ccfef10569e285f98d4272d74ee24e9ca5c79dbde386" },
                { "ur", "0ec8773cdb165241863693c4cb3533ecdc689c6b5ea4c16c2d8b7ceef71b3f03a00f741762519666de046f2ab90fa22623fe11e66421c503afeda161ae496dae" },
                { "uz", "8ee2ca0e7fd9a7d4e088730aee8bd008f9e19b06ff8f252f6ebad667932c11113c3ce9247eb3451465b0c5db50091e5913cbbbad0fb35bad98f6915a3ecb2625" },
                { "vi", "94c149b03ff04fe8a198fb099356192e7c8cc3f67196bf63d6fa04ec34144706afc1b676e5fc8c311fb96e205a0fb574f4bd7cc69a0c0386e19b4833037ac38f" },
                { "xh", "f7720fcf34ad8e72b4bbe0868e7a2d9940d579cad6201f86804c3baf51b29d540bb8e627438020f2d6fab030b57704762730f9fc8677218c28cfae810acedde0" },
                { "zh-CN", "45fdf72a3c90349efb94e2865b77f837ef766f34a933ad28e1be7aaaa113d79f5e2f16cef52c22ec6c94cf2c57e2850c23224914f5ffd3e93a5a12c0d5643873" },
                { "zh-TW", "c4377d602ec9d1d7372659f9df07b32c215512fa08ba6886aad9687d7e1ccff254aaad86034462bb053fd26ed143e3c16431c8489240454048de9662b9da8772" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b67f3b79202c1eea54c5297a39b2c7b88834766f520fb7cc92321f64e592ce4f23607161fc32de07ca3cca5770ae0fa8bb892ba702555c14b0a9652142cc8c9b" },
                { "af", "b507859015c4f156f3822ee374c26524bb9738d0aaca1ef8995ec9aab1c4842f30821d42ee97494f0afc24942efdc1b973bf04ac79a3ebbf2737f14199706b35" },
                { "an", "7c6f23a67a3f471b346429573e6407ee988ee101147d17d1a64dda7e78d949c4074e0bee76e73bd323b5751ca309ce2e098c521521c48a4bccf536d09686cd2a" },
                { "ar", "c030ab6b68bf3df272d1d4288369fbf405ead9bdbffcedc07a83d5959cb75a7f40366786b7de1efbdf7be96710c15f976d403f5af9e8306f257466851f6caef1" },
                { "ast", "c1675b66a28eb09a055d8df324d2c36417a9974dc64cce32e2550c1c270ed59976e2a16dc5129b6c81775a64a725db76b558cb6ad060176547790dc7ad85da86" },
                { "az", "5e667e89a713608c22429f8b0712fb5211db84f6144f59d4d153a32727aa7660ef57b6f08363415246be6bb6f69bbb8f19976931dc61ca9f5ebcfcb0b7830f3a" },
                { "be", "db602e4c489c7eba472f0fa970d91e1d14d07e544a957ea82357a4ef7c09aec0b35517d51a8de6e58f8b8f2785cf1017dc852fe6d4752596400dd9f0e60cd677" },
                { "bg", "ec38007f9c265b584d33be718d38d1f5315d629a79bd04e67604ccf2d573f94fcdde0d6174e4e4d5f9e29ee1566fc43841e933691dba6401ff313bb62fbdec87" },
                { "bn", "4e9e78b5567c1a9f8d9801d2894a91ff5321e96daec20771441abe13be72d60a21dd69714e3f27ca6a291bb4016c46d67b1af87ef7226b6abcd4cfa2440b4d08" },
                { "br", "aed5e99c7787ebf299161644d8f198d6701f9ad2a2a6f4617c3f71fea809f2d9b32c1286c2fdec5e9914ce2f56a7d5ed7b691b5ce0bd9ba5efaa825d6b466983" },
                { "bs", "5e75bdcdb3874de01c70ebfccb6961438463dccbc4432b9689a82af85140f90da1ba8658968d0bebc5d577be6ec0bb9cecb2dac3aabf8f109d6a0300ae2c4748" },
                { "ca", "7a30c3213dd53be2b82574b65ce6b9e0e8298bd0731b2df8cbf3c5731f0a1c4c97d7903863f10b39fe3ae665a433f974e62b4139fa607ab18529f5ff0298c899" },
                { "cak", "316dcef8919190f000e063c3a4837b3b5c076e10c775f92c38c249a45b93832013e53298a6998ae0be6cbad09d74701f917de6db7bbff8958c7a8d472ce46d37" },
                { "cs", "90551150838240203769867a1e4112652448b2263b46fad6681a8f5510f80215ce065b99dc99c7fd9b1155ea3b4aaf2b77ecc8baaa36e6ab1758e05f09a66453" },
                { "cy", "f47b94997f67f2655ae0a8f3b097dc5b0ccf545dc6c369d480e6127a9483c4446bb8ef5e69f357a88bc40a196ce6c6bfb2908aa69ebe7d28c4f1b2210b1a3180" },
                { "da", "94bea90e9f702756ca26711d3b413ecac98891668a2675996521a88e5213241e03944bc0b16865a49579f26f2ba82eaa873b109a75fed65516801d660270dc05" },
                { "de", "99043d7680bd28f61fd50f46f56c61114b4dedb13ce4869c12e96fc835f32e135ea2c81b972b2283134bbace045a0317851d0f1ef7270955eccf5215dd4d5720" },
                { "dsb", "a5e187dc37cbe447e59d244b83148e34045eebffe8866e97180fd7a7e04c3396e6d877bf52bbe212b0db4725e385735e09d1681c883b70120afc9f69ae432cd8" },
                { "el", "4d8bb248b60e2f59579143e02dd026a37b6ebef3aea59e59fcfbac81a8f6478b5680a5931bcb3e190a75a93a99b3d7c573e406f1d9a589528095e2089017af26" },
                { "en-CA", "7ab84d6f4a357fff071b76c9260a796e8b3dd59b22527cb6a2b36112eabccb5a3efa755b9a0b7098c958004d229b28445f07dfa0b723c21d926ae4069cf58bbd" },
                { "en-GB", "623e8566c0aef1e5d73c9837641e425ed7c6751f9011866265111d4f1d1d2cc6895e3855572e2f6ec4abfce9fbbad33cc88786145002ff4898b9ac601dd5f557" },
                { "en-US", "022556da8922a456c8df2c93110d9213a21bfd82e86d900c601688cab0ae1867e86bd64b0790479748ce745a248e5fad065ba9b9b059cd5c56d7f71e70e25f9b" },
                { "eo", "42edcb96f7acbd3ef8475e60346dc2d853e5b474ce5ecb92dcc4b599d5efa16f7773115e1920bf7faeb4d5211749520d4c870fb318f7cb077ad3fe9efcdcffd5" },
                { "es-AR", "5fcfbd116cf21280a689fa4b78f06cdae931a42a9fc26ce368e1d883a8745973ed4c409b7be4e60454e090100053b1f0cdd7f884f9f27d5c669ccc7bc8162f41" },
                { "es-CL", "6f930252875322ed9cb5c991b662b5276a1984f1709be51780bb6651c08eff731fe403c444bf66e787813a1a5df803113d1521ae8e031ae355847f63469808c3" },
                { "es-ES", "4bdab71b6222048aab2b751e37cf0d8cd737dab6565c69695c5857358eeba95d1807060331a59386f255edb3d4c15d97e870281c04f1b2ad88d1a882193391ca" },
                { "es-MX", "a08766e9a2e76b3a5878a58f21c33617d3be63e7800d96186944ac68d74231f027b3a6d430c02755a63920afdc026fe9c146fe6bb360ccd27553f7cc44a90f72" },
                { "et", "d491d7bc75ca94cb97858e5ea9bdb3eee84194dfc175f78495d0723d5a2533b131219df81dbd10cc2661e44f6fbeb26fb97913c6e2b7ee20142feaf40c48728e" },
                { "eu", "6cc85f8586cb054706c426a2abc30807648e20101a9f86f0e4a424ef0892241ce71bca2b35efdab45b7265c1c75bf63651325926320ac30e54b92e99e9e1fb11" },
                { "fa", "3777eef2f83ea3c32bdee7cfbaf09fa8e387ac647d31e8a8ff95b343ad2766a055c9c7c73986aaf557a08780353d32055ba6d238c29e02f8e6d1d9b24c98287f" },
                { "ff", "021b654e48d60a65ed3c2a462822e6e2704e1fdf3b448157483bf00c6257e42d8d20a5c2fcf4eb6058e93e7141c54834e231d04cec9ed79360f0f24b0827b5db" },
                { "fi", "fcfdc683033ef82f13d209704eea3a259380d13175c21908241aa36047e2061ecb298f847d9fe32d44af0f4334d80908298cdafcb1e3a6c5c4b9efc13a87a7fe" },
                { "fr", "6984fac4fcf4241bd8debf8bcb450ac958b72f5d7a6db838206400ef3d1b1135d9b27544330faf95604f936124ba28b40af587fa7f26287cc0265913dd3a01da" },
                { "fur", "21df56b2e173621c9fa46e7a714ad5babaa1a7926490d2cdd7dae8d13897901a637e4aa4ec03afff99af0b17a3c64508ae72cca14421f3c3c378b9e548b36782" },
                { "fy-NL", "d60fe4bd9e56a6837b1bdcadb57912a6b9569dbcfbf330ceea4ad59ee3c8d193e95954c508f27099fdd9297a65609b5179e6d0552fc6763c444cdcc26df8316c" },
                { "ga-IE", "46a9896e97f172ef5df57b379c6a5b03f7d1c09f4f7115aa0dda94aa79f3db305fcc11cc4175aa95db8a35a703aa552d0ab389f3a18d69b4fc1ea6ede739cfcc" },
                { "gd", "1f517efa48dd04aa1714a1d9ae56941524804aa28aa1aded6ac8106067a52741a4a12fb44825d57e8cb716476913f4bf808b11a28b70035b4846aa3eb3360c1d" },
                { "gl", "85249caadbbd36988b3f4866259e4f5281ca60571eadfc182e63c7bac141c314b095b4063522a02bf5686bd720deb1ced3e07eef66318b4089c7979f5e341105" },
                { "gn", "5d2015563154d1de75c2dc586a499c444c34580b0a6a3297e77ec08e1c216ae3c5a168e4be632bff321cd20fa490c9ccf8c68d546e12b50c28a7ad7f1c1f02b7" },
                { "gu-IN", "7c1877f90a7ba133076f0d79e03e22cdeb3cc92cf1cde68d9d648143da81cfa2e5f4a5bd976c5df01099ad0658c6efbbe1596d16038c740e88c9307c93886389" },
                { "he", "2c3efeb17538ddc5408e838833964fb5b8818e76fa6c57166b8865fa65276fceb24f53e10939c4b19e4a628da6ab60c6c99dbd47bfedf58db807bcfdd653c9bd" },
                { "hi-IN", "fbb1e900d48a1449f21dcc30bd0a9572da37a24d2577cbaca358b5325b4a0e5db46cf6c406ba6c155d5ab98d60edc2b96da9b2daddcb3471af19262d956225a5" },
                { "hr", "e3580e7ea226b9246c9ff5708097616cb30e3ae434830f64697b41a7db48e42df8ad37b0fbe5e937915fba49e2cc684113e95c81e8de738db9c9a6b1db26195e" },
                { "hsb", "c64fe00874b18ed6a5fbe2fc9b4b344eba44ee97dcf40c154cead19820cc61c8a35247492043da19b5ffdab98e1e5a3e0998b507ebcb29914a96ce2832da456d" },
                { "hu", "0d943014b6873bcdf2fe8871d692e7621227c411c908d8b9fadd29551eaca7e49abfdd257a229755a5577e7cb7f068d390ba17ce782850f4fe5f69638d73f7b8" },
                { "hy-AM", "33de1624465919fcd49e21d17e314331c87aff2af30f19da83fc3eaf134daf7332be4e9df2f11b69aedc7d37d4b7e4759ae4fd7f44d45603ab25eb97a3d5f98e" },
                { "ia", "61645298c01c5276d226922fec8b97c3087a6b9e048633ef17cb3b6e4f99da4dfb8dd2d666382b417b45a662a7917489ac99999b5dde1b6f18510c9c5d198edd" },
                { "id", "81422b7bfa5edd051550b4a0ab68505476125312e001aae15b0404782421dcafa72aa5959df5f5b92f0cb8adbefe4067909ed6da75f7cf61b451526ea60f0331" },
                { "is", "0b06d7e856bdbdfdafe27bae6d9260caa93f58693b846d2a82879202cb492c33b40ecb813758bca08cdbd1f2e87b8998fdfe3927b9215d41026ded53c43cc299" },
                { "it", "505faa54ec7f93a0e09e371919cc29e71c3c5fa0106d6ded70d8dd4b174692a1fb50ffbdb658096abb44a8c508abc29cdcfdc6039f8bc99a00b6cef4354c5191" },
                { "ja", "30425dda3730c94d67a0a3d34646a7e274d6dc6ee4ef52b2db9eb38e36fac48dfcf61ef8c32d183852c08e1ab52925a9c01e3cef2ed5e796fd296cc67c83bfe7" },
                { "ka", "23844c7e15a26ba8271e5a23f9a2ae5cb5fd3499d59ab3675979b05389187ba63033105f6cee49f238989a346460983685bc313a951347ea66cd34178f04a2a5" },
                { "kab", "fcefebb3b52a32e4d75b11a261d4ef82bb36c094db3f1efbd5146e42711bc9f3c6f220709a8ba71782248f4d51fed2625adcdd31cd534ffff73cde8c6a4e9fea" },
                { "kk", "17259f3af15dd4d92d054daba11d267915967c00fc20a7adcb035ec8638d7ede98e7f9bf1d1267336c7965b1b438fe752a314bbc0c07cf82e3e495bd9dec897f" },
                { "km", "4ae95701526a8e559155710453c00979eb21ad8b78865e195c2e42fc6a7041dad45bfa07733ca620398014b32fdf3bba8d2a755a0ca34a37b43bacbe7cbd4e1b" },
                { "kn", "9b6a01a338f2ff3907fd8a48747c642737b4d59d937e06b236ff7f7368419e901b7de871f7a9592b9c23a1c419ef62fce4de7520e983ee10d3396e3f83a160c5" },
                { "ko", "73dc19797a526cc321821165f17099b300cf10b30899a1643925770a6b6a509ea67568614853572cd58781f46b0a76c6a1e4613a162b819a6f7ae4827502f75b" },
                { "lij", "61c53d0465569cf53f0b447cf9971b2e01ce0f1a0c6b1eafddcf7f41e18185f0cfe0c59deefe27bb34129f18ee14b26e577440538727aaa6da863325b80c1b8a" },
                { "lt", "0abbd3a535d3f4ab6f3511e7e5243e3e618f769eec2c775b9adc262edc1d5a91e3ab836e032e6b984fe9bed798381c4fd969d85422da0f6aa129c966de560da0" },
                { "lv", "3e19a06594d4d013bf028945a599ff53866c6dbad959629c3ac2bc3fe63ec9edacf1f5a0558240f1599c1fcb122ec74d6fe54a2228d9b7dd0047f2affca57f6f" },
                { "mk", "a76bf77586e26aadd54706be822b1146969ab808ab9edd326741ad6633582b982f30dd23e978cfe80ad93e5182f1d361e0a6af26c18e72c7dd83640aad26df12" },
                { "mr", "925881e0f2ee636a297675869f648896a4d58d7f02ab75e02247ac4902c4d5fae87317a56a949e4514463b2f3ecec54f2e47037699df97a9e9c9c7100344790f" },
                { "ms", "2b155038849b3edd31bb18368ff3f907941132c6ce7c77544f43f6dc88beef19a02fab94701621154a90a125dba877c76b617360fffc3b0d0a30837d2b8eb93e" },
                { "my", "c04c88fdc354b890f1bf7f3348894ac90a10d1ea0a640b25bb5b175e99a1dc34a7e9154d1b320280489ec23f52468fe6aca65a6f48ddc7695515eed829a28061" },
                { "nb-NO", "f13e71833eb393992dc3c0d7e561102bdbbbbe3eeea99472e1334d4c1b829bf12024f7ed8812abe0631b8a4d46b178ed24a0cdc8467386ebb5da3dbd5014edb6" },
                { "ne-NP", "b1ff7dad4d6622dabdf2de94c3627946564aac92ec9df5de188176efcf716eb4eaf89302d93eaa5215b92bfd0ad8863fabbc6d1caa7c9b98d796b4a7ef81ba0d" },
                { "nl", "76e0d8645f50da7411af4d38fef7d90c436a6486a45dc5698c8ef99c63f2e9408ddf2896aab3242856dee0a69b83c32a9924f717c0d812ffc639f8abf9bcd430" },
                { "nn-NO", "71e36638b7c726d87b4ee77799c1aa698b78a2e519c95313872bb40aff04230d4c0cb4cfaf9be963e225ed2dcb3531382060bc04f7ed001b352249cebe566b98" },
                { "oc", "cc8686dbb8a7944d752eb71aa617a792d0da6b22596a8c1927209fa3c4b656173fff6883e542cf077f476080eabf4ee701e6bf2064b5d83bfd4d52e8d8852807" },
                { "pa-IN", "b3e670e5103348389405c7df93dbf9e70f834ba824d601c1824cd2583a6098ed8376016fafda0139cbe29789031d77488abeb085d917bec855560f574381dc28" },
                { "pl", "b8ccf36d61f926abd4cc81660cc29bec0c2a9511b19d2676f7e3a7bed6a73ff9f00ade0e84c05eadbe0927613bf0f4877c9d67f03274d0b0c1d05dbd7e9f61ff" },
                { "pt-BR", "544805e8fbf6e73fd7c9dad9050c8d3c089953e1e2766f6b46a2f995ee37063a74aacc45d6b0d774c3f3c30b08a7782fc5ca5a37710c9c1c3fc0ad8217e1d608" },
                { "pt-PT", "2884dcda6a626a2924237fc36c71d38774027228ec00ee3e484a806503c8903815bb4435a3656673f2b8b622ce770de47cd90f859cdb2743793e30f1d05f4305" },
                { "rm", "1c834f731f2c594c1e0a0cb7d83de41fd07155d32a53241e9f8fa4f6bd1c89fc17578406a848b0abe548e48cd13f9654e9e2864f5a2bd69d3544e531913e93a8" },
                { "ro", "26a24d1fb5807615705e15802ddfb249a8aef81e696347a4a8f403c2ebfd8ab34b11c50d715831655fb4985b8b2d0de3fe4c2563d438107ab2dc49f6a12012e7" },
                { "ru", "d91f84233ce8e91f83ddd54c3893109d26717bff8edbdf71ff3b9e0a435713891306e9f7fb444d76b5ca1bf8bc4c7fea99eef52536d9210529f08c8bf848031f" },
                { "sc", "282e2cca6dfe61cf4ced6f9ca39adf72da899a39e216165bfc7620ffbc81ba4e9bb8a33c228266ff01ce11bc00d6c083bc0e6adee204332712e0032a9a09b440" },
                { "sco", "ab95b9e77cd6baab03b9ea5ce08e6a2748066e13866a871bdf47463947a5c636132d36eff055a414707cd6030df91ad732edfc83980df109a81291a5ab4fbfb3" },
                { "si", "e6444d793b9c69810ec8e4940d800303fb43b55033f78ea0543fa9302921c6a5149f2517309c58062ea93dd8aed860c468e7c888695eaa62a98b8a963db5b04f" },
                { "sk", "b22e29f674791484f2f6d7c331f888019e829fcd90231d954e3f3c10417b138c2ab1d4379c3dbc471dbd5c5bdf5ee2136ad15ace07d9973c640d9ce33ee72c2d" },
                { "sl", "bb5345c240c2a19ae1bc3a54aae1342f28926f88d355e765933b41d807d6e1366e5165b2b40976720070d08ef07cf2219e58ec51af772d34f8845ed97df39118" },
                { "son", "5e50489aee29023571da443746f2dc152ef48b3971735db051249654227561a659f9c0268e9b63d0d4a9f132dd5ff8287f97bd72eef117c9828bdb1fe0660a9b" },
                { "sq", "f27c902a7d944580c6695f72d57a974108a761eb597fa094f5ae9a844d7f7b7747775715c3da864a55269fb2a5827f9dd2daab35639b05d16475bf7f1d108987" },
                { "sr", "3b654e571e4d5e0c45b007389d437589a8f3999a968445448bf41b109bb08fa608434704328783e59aa959eca4408a977859fb309cd9200f01f063043a5566b2" },
                { "sv-SE", "eb8ebbcad16f7651b124d94fd146d1de061a36c515deca08f1b0e252e3fc36d8506133083c25ad4fb13aa4aff47ea308037180b342299bdf59ac84fe99385b00" },
                { "szl", "b733b40c9fd96a7c175f710b475c730ddfd8f5aac8efdc1470b113ad9f174e0db328bb4bba064a7c4d9abecb8b0f0cbcfbaa5076cff2680495031c2da6236180" },
                { "ta", "3827e8856bc5d7bbeed7bf8757cf2a0b80eac4ab333174f66ecfd5415dd7084ddff91ce1bfa31b83948e9e923f5363084c5fa0ffddd154da10f736a0d7d65e31" },
                { "te", "f16e80021f815a195e5391fc968270ee9decd97ed3dc37979a6ba4bd6fbf9cdfb1d27f8a715737055cf837d0c25c33b8e7269a3c095f623d2ae551bef3d3d59c" },
                { "tg", "f79d7c1d8873bc85d6887513962643b894823015d085c0406bebcac315b942ce7ad0eacc7ebf23f5309acc5e9016e7db6cae1e42feb65410896adff81b3569a0" },
                { "th", "3c6dee57604fae0888f1191bfeb1715c304c5183e6d8b28a8e17494ca43520770cf38aaaf5e3cc70e9ca9bf5b3bbb99312e2823d67681a31d3f8c9c46e5acffd" },
                { "tl", "6356bde8886ca433f92573e0e488d2786ab5ee17a9ac62694fd4ac72124800bff1ea3668858153ca0b3bb9010e056aebbacd038c8c4def21b0487edd81814f01" },
                { "tr", "e49530ceaf43c265b140b8d85468e1e82d12c5db74a870891480209cddd6a4c212e3a5e9ad26cab676bcae2bd775caa1e3d7a9bdd65ca3e097251d60f64b6a3d" },
                { "trs", "57857674fdae0dcb79322d1af31cc163abfb941fc1d36ee7b523e27a34d67381fd0975b2b1a95edc19c9c53c40c2f8ee79219faf850853395a7d8b57d778439b" },
                { "uk", "b61ab7cec76ac0bda4bbc4406447d33bc61fa658abdfb83a6070a643bd125c1531129c85ba16f765bf82ce9b2e3b42b703ed7d6b4aab6f4cc118a7a7a4940f9f" },
                { "ur", "64ea7bea82f0761d27ffc6e71c2aed28602adb2d82d4fec1b385e77e00b48a4ccba9bc3f9e50c143d21186ff9728aafb20af11f8677a8d50f0afdc70f126ea64" },
                { "uz", "0d34bb9fa2896cc2b7730e78d67e6e5bb9c500024bbf8dd2bcc491a16ebdd3ef6a6278d8d001c3e2a024e4ffe7afed97f3b555cf5d45c756900131494b08fae2" },
                { "vi", "d6b11731cbea8df239a81517a9a76e143fd644fc01e0064a4f348a868b4e4f3b2f5582673371f39c21a7cde247329e470d68e2494b791ec22d19bdf40168acfe" },
                { "xh", "3278bb6ecb408f2956f7a37b0123b8ec6f78d7e3682d0c4fe35b2e4177f13aec660c58e45537b06d2ed1764789f4a12363dbde54a83c1a56c2f35da91f080474" },
                { "zh-CN", "d8dc89f18694e50909686002269883adbf3036d0fd4a5d03e0eb7caab84730dfaaab1cafab8e59f1691847af6610fa770812358197226736f28219146041a1bf" },
                { "zh-TW", "fb114064645ece265e963d4408397e0f462a5438a7b7b2484469f39e4207c82f24da461e0711872b697ff78c7ef2754bdeb6a06fe37e3fe41f8a30e74811b1ff" }
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
