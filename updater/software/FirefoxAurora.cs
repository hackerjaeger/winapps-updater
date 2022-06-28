﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "103.0b1";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/103.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "179092339224569f2004987681bddc5431db829bd9a5d84dba740985ec57ba0ef44673e23c0d7f388599d226578a17ed525f5770d28b43394cec8ac5a5ddfeba" },
                { "af", "08e2fd968d4ea9a83b70c5ef630fdd487db3c429f5356c2cb02df6605946e2c042012c2aa115bea3e21fe3ac49d74f8e228870530582d08241032d6cd79c4827" },
                { "an", "4746a83cfa72e45ecb9f3813504183317e207edb2459340bb7b76c8dd1ea79a53a0c263e2e624f7feadbcae299a8ff67c7050be7ecae7499f44fcc91afcd040e" },
                { "ar", "1eb51c6304cfa5fb7583c1e829ba1202379f953a56bb2bd6e45f9b5f5e60c2e21469493b070e7f578fcb88901d8c0226d2c2b1401cc1098ac5515a6dc0b5bd26" },
                { "ast", "48e669b57f4b93fd133778d962b0782ca94b4e4e0591ba39d2e58f7ed17414dbf90d26573befec665ac8f1045b5e571ac6315c53d2d576c1b5cdfbc6488f94b3" },
                { "az", "5ca21402e4a11535e3aa39f06892dad9714527144c1ef485eb9d95cdab549c5d8939b119f7bcbd1538ea668f4a90647c2907914c33ae2a0a63d780e9205af1c9" },
                { "be", "97c86eb407bb42cbad39132417945dbd0c9a7f6b30fed09c51268a83075bfdade60477115b2c3b501a2fad3b91a33983c5d8702c586f728ea6906c0db85f967c" },
                { "bg", "d22de9612826ac257e37b5084976bbe95925687cbb95b5fd72a19cbb75a33befe46226b8a757f86f2a525d3da37575f48272370a711f2fb2ff927ac5105c8f0f" },
                { "bn", "3f95577e43412a2eaae2d1ecb0ee1d4ebe93ff3786f7ace5c750dbe4f27e4104fdf1b5973405fa1a569690304156dfc860a0e928d70cf04c1b0a6bd54c5475d0" },
                { "br", "4c873851246e02b1201fbb8ab9101859249709efef88ce6f7364462f3da5d6970324d8ad39a8646b0b23d1df1354dbd89cfb6fce887467cd336f97481cd80da7" },
                { "bs", "ebae5ece78fec01e3bc8a3e22a7cc1b54a42fb3be4f59d1eef3796bc2e4a2ada5280781eb900f18ac0f719ae560eed70ec5d81fcb6bc788a211a62760a6fa25d" },
                { "ca", "b00ba14b3349c7c51e78c04bb5ca97bca5f78f3fae3247ab817e27937e1a33b6c7cb7a28c695620f3e954a994dff6a9b9ccee1d4aab01c795ad0a4e24763750c" },
                { "cak", "0101970596c09bc92a16943089b300f7fe061dcea34567b65da939e292b32886f862f425c2e6a8354320b093a689cd042616c0282ed715f1a7c515a0f3758bd1" },
                { "cs", "30ac2a78ff0bdfd4946c1c9606ddb39f5e5e982369277207394e22f4d6051bce1e4863a15b0f53abc1e676e84a65a059d688597a0ff8c32a18fc185f3ac8fd79" },
                { "cy", "ce0c39a292eea0d13c27c90dbdce1db691ea6b2d8c4e3c53dfa39be32407b72c68b1f1bcc94d947a1b1eae900f62ff496048c6085c1615008bcb9960280596b3" },
                { "da", "b02c18b3c2bd90bc9b926e26bf993679456b2102fefadb55d9742360657e78b4c8b716d705065b7cbefd599d1071e67f513200d484f9999795ed85f8316e620f" },
                { "de", "fe7dcd95c3318d8edc26009e3e3732a13f349876247320ee2262fd5e2238698785fc32085d8b7973e4556808bd5c2bca85670afbda60ddbfb59e3d4da39ee200" },
                { "dsb", "41eca5c503cb213e632b107fd12f36d0b88e2b734c8e89562de96d530f1db72840088b3ee677a5418f5bad1867795a144d719b860239e28ac62964855172468d" },
                { "el", "591eeafa853f4349ba5799adc1e12c70aea7ae27d82abc00381eb3b524b9b204d09a05645cbcdf955221159dea7a7cd78876974be97ed22fdde6ee47ae081faa" },
                { "en-CA", "df8563b8ed3876076787dca4ea89b6b752987c61c56f2c44429317b704e83f7b30d7983601d2693d33fc4f49d5c76d19d9b14aa78a403cf9785746f7feff8057" },
                { "en-GB", "08f7699beb6cc7fc24394145ce193b3480439750ddebc047d94d4b3e2a06a9be611e39140aeb3a22e6e250bde1a7215fe73bcaeb36e1e8ec4c54f3cd7a2c6efb" },
                { "en-US", "6e96e35e54b7a2c21230e91a46bea2d56014beb00b38240b6c01bffe8b3b0043059ba344b8fb03a2abcab972ffd1a87f4098eaabc8be96fa2f8edca55835e458" },
                { "eo", "7fc215a8603448ee74820da68896249bece7d018a1e2695c3c20ea970377aa003437a6a345bfe95efca49ef8e034046aa32488fb5a30113ffe74af0ead1fe7f3" },
                { "es-AR", "3d2e8c132eed76acbe84fd8e70d2737ba3c9501e15465afa2c57e8f54053c28b415a2dff5f587c2cf75ddf1906bb1bcb62f699c76793bb1b3efc48f8128e4003" },
                { "es-CL", "f9216ab53323fde7670ecc32be268813ecfe625bf7ef612b9721c72964213954af0cc35d3c5fb5f92c03800c5d5bf681e9b35182082a0703ddb6ec8a7cb2c3ca" },
                { "es-ES", "6cda25ee9709c7a46a6bd7c8d371c973df386a7fc21d0908e5814f316f25f23740db5f4d7c788b67f483c873e1af51ddcefcab24562a30109f156051b243603f" },
                { "es-MX", "5db70cfe9a29fbae546fbdddc782391efb7d0f021b987bfe5d43b4eacb741788cae70c77204846a759d226e7f967243eb337f7f748d004a12e88534191ee1cc5" },
                { "et", "a7dff0e7df888b5ffb5144576f3787264ae857ba4514da506d04f342f9655bf18eaa38d1d9836f5af2435821da904c992abcaef890874c0503efabc3cfa52bad" },
                { "eu", "d3f30c860046d7c11ab320071d16f703cd1aac4ff6515feaaa0f29582dcdee090496f8c3c1f64e47fdc1195de736933238d317ec9ce189a1d4480614ba3a421c" },
                { "fa", "b1bcef002318f5a6433a646f681eb84e278416a00ecb1ac05d5676f4752c59116da3ae5fcf7da5fdf5813aaa1a66f991f96e86d3104ee2a30b7d3c2a0e2936d4" },
                { "ff", "1b38f84b4e57c84839096f44de37c4c0efd5f69a2b4e5ca3476af70e233d69c5f86b268ddb8d0e533ee8a5e07192f676122393725340b9fb1813edbf53c4b641" },
                { "fi", "326d92dc7703d1f14f81668fc97de977302099a7e2a455c52d6f75355ae0a1a4d4d20d046e6e327d965a13803fc45e3abf72525a110f7d338a6dec967e621882" },
                { "fr", "9f8310156a9a81d8de138b319c058386dd9db5f07c4fa3e933c1ba96a2ba65ad62ff0b529668352aae4141dbe06643fc1c38b7bc30787b096c2f64f53b608be2" },
                { "fy-NL", "fe7d8ba4170b959c24518b23a56c006e1176bcc38ae3d7ed71cde8aacbc56e67e2bb27a973ddd746957dd16609daa5e06c40dc8b2a07ad0838ca2c928ba83154" },
                { "ga-IE", "967838fe33f7c276318819e66a0228d8303c6f350d6d2bc582a23bb71702c1afa131fa5c10fef9023d5692e5d9fdfb23aef97c87683a09c1b0e1ecdd3f8d166f" },
                { "gd", "531da7fc7b1642ff0437959f758dde946738f5dceb2ef4aef62dd9135f34d98d2fa64f3ae0400c1d1900fb3d1a17b74246257be1c0af966c0264b3872bd40d25" },
                { "gl", "85192d5d31c5ecd0ac19c0e886015aff7254bb9ce6d5a32adb5e88a7c8a28000b97779f9e5751ce7c4250cc58e34000a94c4538fddf75d59aab3eb6a0b81a52e" },
                { "gn", "532e86290b386f05721a0bcce74f71a635c20f1d1e234552c4188855a2aadcd1b844a03723d4ff2c482c3c50e70149396f9766626c166bcecb01ae2c45c4e1ed" },
                { "gu-IN", "9f8b878854894082a70bb3edd16e2fd18a7c35425f86a15f14ffa4bed80ee3837053413a9f7f13e4922ae4b623e64649e39c2ef202ef9e4f9612913139380fe7" },
                { "he", "229dc7fbff7c6db3c9e12d18a24b3ab4363f0fb4139ebd230ea524aba49a2d4d4e5a30dca783a37f6085580db8bd80384436bf2fa8df6424e61fa9b8e8af2fa6" },
                { "hi-IN", "14361399d6ba81e2e4b204059fc4254bdb8cde0b7ad669b6460a2a3e9a8566ef9d24ad55e993e3bc506810e4a5fe773b5063a4bce4a8a1638fdeea79ab9d161d" },
                { "hr", "d600e1c752bd6ed559c9208452c4ac83cca976e9ef634d6b39b794cdb431ed47fba56cfdcc07fd0204616979b1a08a9cf3dacdeca5c7f8255df9c284163979fa" },
                { "hsb", "8aa3f2c298cf85e50de63e8e3398ae78073ac0bbc9510e4d72163aa58b7fedc030b86f742597f6c86fafa7e82873819c62a004abd1fa5ea8179b05ea5c1d01b8" },
                { "hu", "736551392b75c15e9f7ac5a362b8c7582eaa59e2d00d583a2855cf62e217dac50c9ba07b5f281a67c90c198283022cf21bae216e8e9493c8790e282f430b4c67" },
                { "hy-AM", "38f980ddc1aebf1d9c9f7f89ca4b27f835db6d40b8cfc5d22cd1c48f49f8f9aff392cbdc115c72f512073a1fc6b631822f62868be513c3fc6b7e93e7b3a4833d" },
                { "ia", "e0dbe3c483c8c863fe1947e5379ae9672824fe71e18332d82ae3c7076f1aaeb218b0b27e02f40b14b692f7bd4c9b2a6a89136e0c50afd2a1a39ac88acf48cce1" },
                { "id", "cce906f6f59a1f25b4b5dbd402ece400c04124709a918cd9465769b8b42eaa81579b0203ee07cb80c00a270161fe52b7adbb6da2856e745803b38d7619a5512c" },
                { "is", "9008a73129cbb5f2103081062fc30cf2b4e1bc077618fa96a9323a4c61e50139b0deba17d112e6c1ae363794ee165709f0d575a733b896f253dec4c8d80d84bf" },
                { "it", "7680d4d89dca4179f9bc0d3dd8fb738e778016f38d7c685797af2d085cb3047c59a724285b7b485ce868893f4dfb587c507a05cfd3f5138b014e2d770777ed00" },
                { "ja", "7616849fb93faf8cb1648f1d2bbb5d797d02aefd571ef3c9b2ec4fa17f2656431fecab635a5d75ec01d13d3ee9427150f4d65ca6d9f310d605579c8da16564c4" },
                { "ka", "9b6be758100d895e6a9e396a498c8c2309871e5999ae4a95865e81c33363a31559e25941d1182e0b595536175e9486522f5f2243b25549c3b2b39c2b55d92e12" },
                { "kab", "c6b156e5b031ccff94f47ae96242e1b0ec4ead9cb1124e4cc8d4fab3ebf1bf52e263fd6fcb11d27bba9ce566c781f0928e26f526f6f807b003cda748de2d3db3" },
                { "kk", "a8a81a9d914389db3d5647aba4a76dbf6e038f0221936336e41951dc9e0b4089519d7629ba35f698d8b7b3ba05a3b306a7385fe8c93ed9c25e04784a1694f0ce" },
                { "km", "4d8d5f2fbae988ab15abbb9eede9ad810348d2b782f68f50aa9cbd0f9535c63d6d0a79290eb8bb35621c6f4a23ab39013671d5c099a895c683db3d86f25d5995" },
                { "kn", "29ac375d01e2dcb843af0af82381767ace1a674283cf7af8b3ee3da9e75c02808deac85847e421e93c021fb0eef57487c4dcea52c1094a57c027a7c2fe6f6117" },
                { "ko", "1529f2e18af06c6000b7be3a716b7f6b50c99aaa499bd190eab301da11f8a2d3a0251e81e25d0ab46548cbc397fd2c9711b15c4fbfe1d1dac5f17b67c1a2e616" },
                { "lij", "921871560444a765bde1c8e76686d62f8d52fd5f734fe28726eba33d805c50fd8d1c7ce690b50ac35dd9fd3ff6713a77b0c1969bf1d37de245ef79836bd91f45" },
                { "lt", "b85a62989fb542a2cc44f1e4de0d931b29fe36688309aa9059b87b6ce1948d0b5f8cd114a694baf9883bc133ed8438eca3e4131b0e89a9f163d2962a4fcb0bbb" },
                { "lv", "717c10e0ef4453968ae6636c169231de5792938e5c9937e20f32c181303d7be3a66ac2d61096c3d967b397b8d53a9b77123cf7558d05c2e32d35e4ee6746fde8" },
                { "mk", "dc6e562bbdcf07473104fa33c30393700cf077c0f1d102c3cb70ca500760565138c29d509040e64d6b5af7dcb63a4ccc873c3fd4d0eea837382ac2bd3a0f1f87" },
                { "mr", "c510a69b204d6f1a990bedf73a0a1f26cf1c5123785c8cffacc8d9b006d6918801c73943bbaaac1c75f0fddb0c0ab0c1803de10a5e870c533b921cf1a8016e70" },
                { "ms", "63305e96bc02e8b7021c8fd21f1a0d9c704b7a1ce03ed74b9b90f57d67053489123200a0fdbf8bf01e6c019bac22a248bed7755c59ea5ca807d110cc11684b85" },
                { "my", "96c957e8ceeb303402e1c83a7fdc99fdd96b19b3efc57bb25a2b3ecef60688295d8028b9373105e19db54da02edb5d03d058e7f581e402557c01000c411f8eac" },
                { "nb-NO", "8a33f3af59422d15615c57684a9a87ef35f0243aedf557507eb65b64875b22b0c73336edbc876ad086c07bc6932591a5dc95892a64c14ff35e53bb62250493f7" },
                { "ne-NP", "1cc6c8eb87aacda8f13bb8b5634da7cb00294fc8c8918de4ff55a4b0f27e9bb239ebc321b6817679ca774a378c18c2eed25c035fe51d06dd9c4899eb417d996d" },
                { "nl", "d5829cd9bc1177f49c2dd6c30bb7b854102e8036828298508c35f396aad480414a8361812e8f68c342636fb9fa0521cf5c3e1c2e7e573c28255298e929e854e7" },
                { "nn-NO", "6a83be15c11179bf7c22815fd4bbe79715aa828e2314e052077a3e055fda35ff40faddc4e73fa5f3a04eb8aa76bf697f5356627da62dc2472c357edd4dbf76c7" },
                { "oc", "382e2dec40f89fbba8550f921e20c0ff6ecc7ca5d9654a8931ee2c7ff8e44166b27d2eb6d8c4eb660243c21129c45ebc14609dbd486ba81fd2e0357c4573a67a" },
                { "pa-IN", "04ab8696098dae2818f02f1e8eb6d993c2c9cb869b8c509e00fc81db83a17a5eb0cbf67c95627c0854bcd2a5a61935dc360fedba112b969d34f14a538c4c9d2c" },
                { "pl", "5105a87244f6e5a0510b09fd6b74d3b17d7d8289c21ece54509a74ca7f8908f8b8660e9cb006136dc3f165cbe19f89efd4c909aa3378243b4b83a2910e4ff098" },
                { "pt-BR", "d4cc6e96f28c84b24b58c12a869f6406a90da47bc47112d8c8dd52c38cef6e1016610ee4d639aa81ac26277752bae133bc40fa1832bbe307eb04584630111d67" },
                { "pt-PT", "2a7f43e119d77c995425186be7d49042b4bece4cb5214861ea49935bb379bd779f7713e2918b97688fcfbd25eedf20a4d66681e8f71fc850b2e26db19c0e573b" },
                { "rm", "e10fa5c74268233719fcddad30a762a63771f0d25744209f47e4301e9817ad3c7b57633904e34431358419b9f08dd28d745640e9af95cdf94b5d34526748ee6f" },
                { "ro", "b99f7f23c6a7098cafa64bc46dfb1721ff645bca6541b1d473a06ca7d7c13c33bed14e5bb4dcc634e81a280c7b96429bf4628ddcd798d55b4f4bdc05d2fc2507" },
                { "ru", "ffd94de832966ff56ce8814eebf5f0e57134be1a97c76f69dffebbeb16dbdaace677efafbfc665a56c145ec3403a164a35a5b7b4e0e126b6bfa612920820de17" },
                { "sco", "341f5b2149ac587d040f8a743dcb29ed37bdbc4c8ce9b0e4792a295ef90e292d93bc92b8d484dc5a03cd7464a422901f78c46ab6d9a16d69e54661d9e21be294" },
                { "si", "2e2948c08ad87abde41af56016f39f6c0855ad0f49ba4b0270200449b30bddf494951fa6cec779107ab663b48cf32db7392609ca08f93d5c1610b5edffdf9d23" },
                { "sk", "e019230806e76b0caf56a9ad0416b210cad21f2baa1c4f592c72d43b369ec1ebaf5f667ed615dba151f6e58debc20bd2e4ffac69065a8bd42d2d4fd2363a7a94" },
                { "sl", "9e81beef9cedf1a8128d3eb8e915fac9480ab140946fe66b4e9060ab79e3a8a0a5f7132fa29db63f72363b409cf2df8d14e35179f0062e36d31186e2326e2333" },
                { "son", "b36f03cac29050b6069a4475c62403cc68167c66be3d753e9ac987c1d4924e98d09adfb62e994d5078322f4b6d8427f9c5612ea9dd12a605f9df152b9756cb61" },
                { "sq", "9722dc712468fd39782bf8bb07712595cea3e92e5a2f0decc7303427ea85e1304fe0c1400f0e73be244632fd0d489f839dd65f02d2c395acc1f4bc4dd9660166" },
                { "sr", "0c983b0e817b29ac7b47f5546c70f89b073a7f88611f4f59381e26b85f1a1dba20159aee049e8f0b8a2fdf63caca9bc8b65e6882c1c1dcbb6c2a9e02d42fcb15" },
                { "sv-SE", "0f1a93f3f23d9e3fce77c720d3b68235ba025fa5cf62a0667e049dcb1ef9b4d6eb5224051294172091c064e313b2a789d196a0b5cb5b86507331eb1365b81dd4" },
                { "szl", "8248befbdbe20c8707ec82e54217ae5ce12fca4293546dd8a12ab045530aefbc94c1a6a6f620e2f0f3bf8865eb3b0d605535d9f06bc9d9f7528ad79dac5bc838" },
                { "ta", "36632f28d165aad967ca762655c371e5b9e09e5e015d9627e42376c97dc9e0fe32017fc929c682a84f9ff6f73fc75dc714fa9d9816ef349ab139331982230697" },
                { "te", "2953243e8ebcb35f6dac5e9dfa0b2a5f7c9e5ea168437e8d4abedf20d9306f3400c3b24e778b6a16a0aac814da14b12d568e7c215a805c5ddd127f82fe6205bf" },
                { "th", "07925956198e9642ce17aada080bad3bc4ead68f87c630cae78bcdbc09db9265446edd6de6d7e2bcf0ad774c5fac47c631c892cf061456b5c2711ff5dcaf1cbb" },
                { "tl", "114be55c45ff21a38d2629929e38a286f14c6f838fb6ca3d63bdefd55902bdb1f4a88870f290c521db79f18b5429a275668dda002b4cab1709a96cc01598d1de" },
                { "tr", "785edc0588147944d146059f83fc8570c62936ce04584679b94ec7a196ec50ff2b5ef14d76b08ac90a4e5afa08f4d748383494488e1a9447b31ff9dc058b6e7e" },
                { "trs", "9689b1dce2c09af81d809d3a40a6fc00d0c538628876a3272f90d18edeb0365f8ecb451f6960dc402866e7f6f0bcf901e5e85e637b27a9ac1b6dca3710c63137" },
                { "uk", "02eedceece04b390b87af69580b5f50dd1ceda148b16693ce3e5177ecb54963c5403f8d23d206366ea8b51aa6242c8ed454c59b505b4dc35bec29d4e82bd2225" },
                { "ur", "6e44a9d5cd9b9f0b738ca6fe1e0d5229cd0525df9ffc2a7b81ad15ef5c73f3c2bb26e41291350ee3b2765bacf9e1b3c98410083edd84fde77c71e0d482536d26" },
                { "uz", "b115292efb8fcdc60df498607483a9653dbff6d46aa363ae13e7d24e0bd55638a7d7defb311fd696d6813009d6127092d7e6c2b62dc6346a9215a783a6909b39" },
                { "vi", "232e2156c0a36c6b03097ce88beb00d413f7385f9749557afde9b2ea219b6f507b9ad3208217499053e1038315c7898b5532addc8122e4773cb32e5312bfe22b" },
                { "xh", "3c159840ded54f9cb8834ffe64d92bea4e563ace17fbdeeefb3884fd5daaa2df7d3c046616c188cdf508053742dd2d8ed4b263e40d7f1ef3a33fb9bedd5168e9" },
                { "zh-CN", "1f33af24d4e2facc17f4dc3a73ed8d75655c47c898d5e11bd4698fe9f033b31a92e0e405a1495566101b28409ff8ee6c0c79b748babb0d80ca22f569ed6ac1ec" },
                { "zh-TW", "ec0b2daad2df291f63933bd67196fcfbd4db118f52cee86131a436e14d3d51bfbd678bd9edb9efc70b5e35f4b6454392a5b0a579318d10d876458c4d7c396dd4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/103.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "17e1d9d43ddbf78cf81f9566a0f20001f41a33b964546b6278bd0de692124e04145ace73450c26505cdbc772ede00ebf8cdc43aa52da2936eb0daad6559070a5" },
                { "af", "087ed06604aeeb1aa34555b0a243b364f7ba7aef3850472ad9bbc7c09842ebbecdde2e9d8be1c70f56f517170438a55eb1a2064a4714c4279af4b34b11b64292" },
                { "an", "1e031f950f3671d7aa2e9591d93adfadc3b2cc22e219b0deb0f50c79bb55f3f29a78fd7b879129b7f1ecc3a294a80067b7f29ccc51a438fde5a192ecda341cea" },
                { "ar", "e1a2ed33ca1424b7dae80b028b89c145f5f19d16f0791b5760a360fe73bd5fb7e935f0f28f73434ef81787df139377af5acc77590dfc4dc838151a59dc2134a9" },
                { "ast", "4dc9713cf04fc44f959bff87eb5f6406cc494963e81d9277c907f046d6928849e94967df2ed0ffdb928d5df26d14329b832b4f2fae437bec9d369df213f4044d" },
                { "az", "df6bb07a59a746da2990da9ba0a5502bf77722a188939fb3ad379c21bf28eef3b28722669d640b3ac12e84cfec97458762f114693778fe340504a2541e6c3461" },
                { "be", "6278c75a488f09a90a74ebd0c616a8e39e37fc003a7fd733d3dde979ca0f2d8b110c1872f7cd1142cd9df9d81a0a78eda89de7084854e06dbe08e5283eb33003" },
                { "bg", "81cf132e0f32e6570d0d677934289639cc80e729bc6d3a4bdaf086624c0ff2ba3be33d1c579624a98a2583c08ae0d858340c77dde772dcf783e55d6fddedca34" },
                { "bn", "0d0f0c719a7a72e4fb4a7a332f47ce609d3689e69d67e70b42d41f940753add3c379cbe57f42e470293a6b026b979ff4792d186877386cf38c94e5c18ad0a14b" },
                { "br", "b31647ab61d6fd94abbc7a174c360a9a8efc5fb316a8c8dee9de0c330b36b03425f9bc41874028f5937ad26bee68fdbb2e1253a66973dbec8fa65307341fbdd2" },
                { "bs", "d42b3fdb9aefaebc741b6b372027a64c9cf2ef451efb3760df26cb0b30cc157a40e2cb2f696848ee69c713fc7d70a2fc5fa7025b247cd90b82a16434dc2eca6e" },
                { "ca", "3c892edc4c17dfc69efb2e44e5f71e8ed771c305065a9f558a6527e5d22a5e5868a6a575e515bf62260f70da291bc3e7b403e9907cd0c8c64d309bf690ac7c2d" },
                { "cak", "a2e11a6f7d33b9e79230dc6b391996453b356bdf7cf0f43a6338f27ecddaf0813c1d7c41aed865e7c2ae34f609523870bc67f33d38ccf79fb41299cd9279b253" },
                { "cs", "076a011fc3808db483eb584f2b62c9194c571bcd34a4fc1dbec0aba8670fdbe6e7235256848854724f90c708c6a4f5174c9fc1228dad990a386971c9e2f8f97d" },
                { "cy", "0c7c6065d669255dd23bbfe1d8c189b5ef53d2d436ab66f0c69d6773e173820340bb57f524342cc2f8298a6ffc57dee5ec7d3d0bf98e454baccd7c4ac629a177" },
                { "da", "2584008ac8c1b808cf185d21ea90516c55125992cb8a35287ab5c99ec8be1de98cdddde518b261b12cf5008f6151d86ee9653c50ce5638b18bdd4e229b7b2a6e" },
                { "de", "76c4d514a44c7d8165a4415aec19efb8987cf87833426936efb22c2663d618f26f538887cfb08f71a39ac5da87cf553a4bdd5ecd24562dcdee51b7c3e60f0434" },
                { "dsb", "db5caf4ef6683da7c6dcfd7ce2fff8da68f159c4ac255ee1a6703be846237a32b00c0403e65aaba7f8a42df038d383a11875a40d23916d82f171eb8ad93e7838" },
                { "el", "3e436e8dc6e933984e76d7f5b54c9486533605cd33eb549501b9934d3e19b2c96eb3e94f5e61801f1baadf1f8d1a29fba75628c93baee319cd2448fc24d1d9e1" },
                { "en-CA", "b652dcd690e23ea705960b074826463bc8cd4c265ce895bfa52a3264e6596f1ee30453de560b8a62cc30f11fba46365f39ce456e38e21dafb767e2950da38757" },
                { "en-GB", "b09df7946dfdb384492527743da3319f5e2a66c122f18e85ec67aa078c95382e596487e90cbd26f01090a0de691a9c51f021346f40990b4138d307d0d9f2bdde" },
                { "en-US", "499bdb3c8c08f16a18320494cbdf56230c2c4c4444e35602208ba6e0981f0e775b4e95af7b3ec74f22ffd5fa3addbb74a1e4aef48856bf4be193c860f59b8d92" },
                { "eo", "290803c54f7795404d0c4f7bf485aa7ac5df0f5624d81982a282613965aacc0402549032607773c75d220d5f9492314694010aa60208becb63eea60b307e467f" },
                { "es-AR", "f87b490a3e55ae527fed40aa7120cf6a80dab2e2730e969ce86494d36c58de6715faa0fef35e38bf93578bcf0782261ae6b9d5eae7c014fa6107e36be0654e4c" },
                { "es-CL", "113b373d9e0f810082edd1a3191e5a17cfff21fb35eff357e97144ead1589396ab9ee01fda7a748b1b9d7b71e161b3e5f0be8410999bcf8ed7821c321e9baf42" },
                { "es-ES", "0f79ed15bad8017eef01d2180b9bf1c3828e85a9a1903f19377f00fbb238d0746559f89e0b89af045138fc4d225831fd606b049a7c27797fe5717672050cd48e" },
                { "es-MX", "c6645ac93215103883b5238af1e07e152e3c7d3e8462ba50d07e776c0ba81d4eb9153fa7db16a46354e1454ecfabcd5b1140cbc22445180cf03a2060a3d4f162" },
                { "et", "cc3e3d49d9da7162a53fe8033a3fb997527b34aee00fcc08a24e587f5734697344f2e2fe9161a7eb9ef39e3b225b3babf453cd66d00e97fddb7b542f844de163" },
                { "eu", "aed03960c9797c7201e067a07ffe90c76446d1b865ec4339595ca019abf044570694becbc861539399be2d7edfa6fc8423e6a709b25408a50c0b0a6747d5b708" },
                { "fa", "73ba4eafc88604db9098566760203fbd7e9b2eea3b22c270e309ccb25e2bfb1b1b7c8003fea44e40eea0468a5102778451b5c2ccad580738d5513c87dfe2cf24" },
                { "ff", "17bc3ff889bbd1395f21c0aa4ef67e5951065083a93d062a65862c57f3aa5a42d2b36c013eb10c056b6a847493134b4cba3f657d2540f0feb581ddc9a012833f" },
                { "fi", "ad725a6d08b6b3a607217fc88691ed0396a9c9c06c666144b86005b4855f9bb60b3e8bd3b82ce561282b27e71cb4f0503ae7453e27c358bd7d13e47d93336c25" },
                { "fr", "847f5db3f4db21570e998e657501186667f0f649c1d4a23f5a0ee9974dca32fe525f32cbcb8870b54a3cc4acf1c210c7500fe9ac727cdf3e888ad3b9cec0b37f" },
                { "fy-NL", "f670afe33db8b4372b426293b3a7634e6d18df00798f888efad5a8fe625475677cb6da6c9d3956fbdbec935aa5d7f344c1f8b99d0c87a21e86d3dc61c4b3bb9e" },
                { "ga-IE", "0088ef6d9fda102e24c6039adbb9b1e7bbda0ab875f512f4134b279caed7f1c3f40cca03cb2545039970db0239b0c66665f86c8dd2469e24cb331b9f579bdb3d" },
                { "gd", "998d20e6aecddfb53c68faaa3137b51a64c25ca40e59f57ea22a72c7602eddd366beba21be58764098652428986d64817083f1b3f18495c04b5ada2c08c54b0d" },
                { "gl", "843498ec3da1e04dcc0d0dc3603b70b4c1524d9472bcf0f7e3bb5962efad56fae88c53b5a8555ced2fe1010ad322264d1988319693c8f8be581a8ac9b0b209be" },
                { "gn", "417dfc2da2b512686cbae90e489f1e295e30b803852df6de62a0fe04d4446a6de0b147c321644c4d5fd4c35f04720814bc92209c5ecad88c51c021a801f02ece" },
                { "gu-IN", "0295cd9d6337286d2af26ddafe6f0d08413e78fc88f1122c363f4d81890e7946dbd3971e48ab2f26107159682fb270999267d21c6c511e4fbaa7f5a4f645b0bb" },
                { "he", "b212d3e991b703274ab0d74b2f8b88f342b1f21cfd8bfc26badb765264f6374c5803f727257fa3594057f23cdec71d2bec2e1bc3ac703cadf631ffbffd70aae7" },
                { "hi-IN", "18640a5e5bb7712531eee054d775b63d2900432bb2c763718a7e168aef27dada3801884f04df604cad21f300b17400741eb0f226629287e96e1ca3483031fdbc" },
                { "hr", "e367de9d6cfd2910c25558d925549f7ee4cfb444321c3f8655057d097e193497b30155d2c357f5e2643fd4edc27bf27bacd90ab73c1ee711868af44649ebf782" },
                { "hsb", "20413473a23f923a452e25efb35f13982712ca07cc2eda28acb089e511350f7c9713b27f0bfbacca68e7f936bab6fb23a09bd8d6cf4135dbdc93a713b968b442" },
                { "hu", "383608b08ac34e6755b16775b77d338682a0f0022e5d2c22a258d9fd32cc720eee0a7675ca276b39bfaadb704d964aef0c221ab30a48bfb80f2a1348a0995e6d" },
                { "hy-AM", "6dc394ed03d6d6f780ddba5713c6c7c2c50de22d43c4b609093c0ea0ad5aa3bb69eaf5e7e40bf939b871da56d08db19567e20050c91e85b3332093c1d8392f83" },
                { "ia", "e7119156d6e5682341549c4139d9dd849834b2ea21c53daf18d3aceba3ea6cd019454aac50afdbcc2a428b4522731979263ddae2bccde4bc0c869865bf5acba7" },
                { "id", "aabf8c29f21dd4d5d16dbab7d3512bc13104a8b995142afe3cdd0a18a62be7b0ccca98f5976edc6ffb73445dcb02d0546ddf99a065c2fd5ec2105f9489af99a1" },
                { "is", "dfa88510d4017102756c4879fd23df067a55acc3fbf08ae7b07947083001d306177b3ca14ba5848f94e7f2579ce1e658d1e17122d2456411fb81b53daca8102b" },
                { "it", "51d9a4100408ac840d71d8a11520d3ce8d680d0d2da102cb50d525bce216ca2d47f3c47bf069ce6fbad799b6455f342a11e56d55389f835f1700cfafd68f361e" },
                { "ja", "39ad1feaab63ccdf7beef7937c87b1c326f6ac20ed7cca98de760259f9b73518a862d4526d25c6405b007fb6043ddf2edfb64bf880459af0e1989a75a48fbf34" },
                { "ka", "456ac051b8c2c50fc464ac8c6fc0db44732e4cb192bd033587af33a0f9497d1331c9f40af439e7983cda3f9f8af4f95598fbe8f7fd2a578c83224556f46b20ac" },
                { "kab", "d06b3ee446d0f31828cb28f71e31033522d19e31e8eac38ef77c16557030dea9164dd7f852f4f66bcb328dd80b89c0f875f4865748bb0e93249dd8cc0f4240aa" },
                { "kk", "6cd6bc3e320b0ca1edff6a1b2dcf1dae23bf397d7503ae00fc8b671009e42992d85c800d218dce2bc38114e96ebf45fb6f73b971c20e361196f31ac8f43189ac" },
                { "km", "00109f613f10413b9c248f76814e3f73d7120751ae25f8ae7bf608b955d037b1a10e8345568cdce92c24fde1656fa48ba861966ebdb28b9c093ceb6640e64533" },
                { "kn", "ac5c02d99a4b5c5474412b9b0dfa2fc10ed8e7dcdb783e4c34f4dcd92a4a0c8aced75d2a31dc20855555553ca521aa26cc3bd556af03393e382b0ae6133ecfde" },
                { "ko", "54c04ff76853cc90af805b1a21e3d1f81997326e969bc7453b8ac37d4586b29e9cb832621fed12c6d5d623b1f8da2ea0c7af7a2ef05f38c96b2e09fa0cc2a051" },
                { "lij", "ef52b393ce452bd27b08e915c2da41c0cc35b20ae45225267626d0a7904a23adcae69d50789b88a5641a4fcfd54198b6e11692f543203c4a2a7a3bd3907d9c0f" },
                { "lt", "2ae55cb24f1851cf1ea3864807c96c12663fbe2f2478ff6f8f8f59c8738d27db7f67bc5bec53addc5bb2e20ae9a5fc415735644ec8a71be1eaa39feebef53976" },
                { "lv", "bfc104d53958bf07928bd628ffe261bf03d25bf7bad209e3828ec8ace483f6e14441ef229e10519397ec83d8934f26c9d729d3414c6acb5b372f69e3e054b35b" },
                { "mk", "c2b7ceaacf1f8e5b19c77bca7bf292d1fc31af4bd62f3b0a22eb82b5d380f94160b3abd91d2a4617856eeab05c65484bb5ea195273142faba0bf04cbce1569e0" },
                { "mr", "3e3f6e5d32cd6f39c866219ec59dfe66dae2aa6e0bdea8ceacdad45f8d4baa15dd2d987a4115c353dd2afaa3dc59642569b647f53d1629ffdc52f00158eec9dc" },
                { "ms", "7b1b040c3eb367c4f9088b5836244f01e963d4dc8cdcfaa2258e719a1b4846fd61d60eda5af016fe950758a1fd736806c490b87b3f056c04da53ea50e1fd35ff" },
                { "my", "6558e087d6be3300569b78248e5ec374403b64f2ccb53564304bea469ea5c3024b5de1c89d05ff59c23838c8f3f14ad345d788be8c6ccb4c155f4aee207fe170" },
                { "nb-NO", "ede84802e7d1264b25b88733a6c429225af509d7b9701800c90836469317e5d3b12e076a7221eea54fb0ba9a9f040305df859d98038c6193b6fd62df2ed72385" },
                { "ne-NP", "c481c3e825661130a8ce266edd53eb364f01edfab31cfa0cdd8364e321d709cce4f46999272233998b995c18484ada830a57e9612bbb0a15c5bc5c438affab38" },
                { "nl", "8cb2aa5250c3943f78ecf0269679f1bf4cc96c8bc7793156915951020cfeaca0f2526dd81ecbd0bbf668eb2db788ef2dc1801ea77c316da5f6e404adc0ec7524" },
                { "nn-NO", "49fe58ec006160e35265bde4233492927eb2a373bf93f7fcf10b69a8a5ffb49fb9439fb433f7e2f19e909c18e746115593dc664a13ff0fde0ca90c280ca67952" },
                { "oc", "0408ed2192a8a127b1485e31671a26f14b65e9d8737a8c3d1f009d0373496e75b46db333d4da6226ff10c4ab0bba79e7f2d03cd89b16299ba9eae0631cfb3547" },
                { "pa-IN", "93fb7cd666eae201b5a8bbffff28c87dbf938c0a817cd55c212523d75d2ae202c986d829a739c60ff19d249d02a57e93a3c8154cff9851aa7e2e6679e1a9fc26" },
                { "pl", "ca4cdf293383118ff581e6b791a0cbf4403b180e12f0d717b3deabf16c7317159a090d3db7008bcc45d2f2e61159876d969199daa1526af7c57fd75a90158243" },
                { "pt-BR", "4e7162b93a813a651dd167502b4ccdd0e73632f07cffb2b9704d3836c7c7b01e9abfe77f6aea84ee72b89f45c72afbf4ea983165deed4dd1aebcb393d7752816" },
                { "pt-PT", "493ff0f0c45b9e4ad698ade9fbd064eb1bb69ba6f555998bd1feac566712b1ab27220eb455ae3a5278ea65e58001121a33d46c7caf08e1470cc3d7baba709059" },
                { "rm", "d6207337f35dc3bc2abc65b67d2252583c376ebd0bee88d3396f0cee52b36996fc052fd0fa6ecb6327cbc08d115ae55699663ca09fece2fbfaf88333ac554229" },
                { "ro", "b300352b385c40356a003a893b5f1204d6e562a10e28ee5f03b1dbd81453e671c8e7eb7bd341f9ab38ae02a1893b1a51a94016abfcfc40ba26dea8e496230192" },
                { "ru", "fe459a04c86221f8ec10f1b49b7ae516677d56bba9e5a1f29a0ebc9d8bce6855e0e29d5924218bfc087d81ade186b7a1aa673f2617e81b65ef2c6824674fde93" },
                { "sco", "0e582886836cc6ec014a59a31fb4907f23a34ecb372eca5ebd0cd61a1f9b77c640e5841db230a08f39c2f064fb685ca6911193d4c8d1f8e83b0002d63ad91c01" },
                { "si", "c8c9f688dfa435455f4c2cac4dd1d5d9088871f7b55c34aeae64eabe902454faf38b669261b1cfce4c09967400efa6fd4fd16fef3eae0c112c35f9666e6a6946" },
                { "sk", "9868b656a538bb41f7e214bb7ae36f43c3f7b647dbffff4dac7119dd0e67ead04af9083e67bd67c9ce5171b8bab560733d8fd832c533c646d0bb3bb51bd3a319" },
                { "sl", "ed8749e060364781a033f94fad03c8db2f0bb64b6c204f37cc52eede5c2ab0458208b2f1969ced117e3d2179fd93e510a288b0e086d2d5458a3b72a5fa4a19b7" },
                { "son", "2a2f599e4905f3375f8241e71a1a8932f067fbe070a3532be4ca2831b0bd8c230221828e5b544a187f4ce03e4b447b24095661b50e4ad1b5a9d0987382e135ea" },
                { "sq", "a84d1b9dea50c9a53f8290800a7b9c04677da0aaee079770d862c5a56f76a286ad5012436130c3ad8d690def250637d845e4038623b920158ac25d44f3df442b" },
                { "sr", "97fc8f4560e9f817c30b42fdee14d624de4286bd10510e54b67e2c321d29ed6c3ce29306daed5a30fc7c7e12a7e2207dd9be624b81af31232fe3a01a8926dbe9" },
                { "sv-SE", "db1a1cca665465640cd43946d082a2ec1aae622559d5f1e0d49b108a2d43d0a2908b4ca6c7882101d568aa201b1d870e1eda42b99184c6a0a3da2041935c5c7d" },
                { "szl", "0086f69ff3e40da5db819719074849184d34156d6924ce6ec22be7fa7202d8c5a6586e69b21c69a442100f02771079ebf015454216542149cacd02d158ba708a" },
                { "ta", "d5e06ef90307140760a5991800c9ae3aa119fee840abf138ce7c68e5d822fdbf7bab15cdbc7e43d69111ada1de9eb0d2177cc907e1f2d3f2d7b9cc4b89cdcd90" },
                { "te", "2bc658b36ee484fb6e3b6eee30432cbc0c9c6890f9f3212ceffa195b8928a6b87e73a5a7d2e6c0d0fc24bedd73f0cb2526941efc855ac9078b922153552ddc8e" },
                { "th", "23f636cce170b2bdc7070c8137a9bd93d6df7811db3db1aabaa5295dd545cd960e3a0ea1a2fb9cf62288826a5cb4b54531e3c75f830567ad14e2ac45fc47ebe2" },
                { "tl", "b00fda08a774651846405e3a3604691f232b2b7bf3a11531b3d59e5a06dac0b7b8c6f0b7d2f385ac004090035e75c65d19d2a9cc4be2639b3f7be1e90f61aaf7" },
                { "tr", "950cacb6b53c731f9d7342e4ebcf275cbc7450c0c893c171b45ac46e052bff3d439fbad2e37fdc36f845861dc406e8e622ec32bca5aaff6fe60d9eb9f08c9362" },
                { "trs", "b604cd90f290f1f73c3e016e36d916599cbed86bf15a23091d6437e06c34b935d759529f80e92452d8f4029a850356a4f3ceebf3631dffdbf9225b43cef811ba" },
                { "uk", "eecc1bc1a08d54d98a42a250af5c2f170146cbf6c92e94d36f7d6ab56b86549ec54e234db21e93c8c19a24e1fba4ddc447ce0754f61c564ff2636a396f35ef3b" },
                { "ur", "aa3f6e1ecf6c7002e233e41e692313d6c256d81e0c4c51423694038c3e2c18bedd72985ca90d8f9e06f25d7dad0acd9c07418bd9b0f9c2899f75d5fdefd89e73" },
                { "uz", "3a4d535f752d5ff8cce09a92ebb2682c5dd06efbd7c78ed1d95cf20628bf6ed4022ad40f7f620e06a273fce1f70266ab3b6ccaec6f3e187a33159fb31a30d694" },
                { "vi", "efd96fffafcc406ca69352fc29ce8c5dea9394686dbcc59a2db7cdc28bd033edec29692a25567fd1caaf2275f41edbe25582f551f42a2316e9ea5fb303e123bd" },
                { "xh", "1b5a02138bba217a69fffd6401915162bcc00ea0919de3fcfe52cd83ab9c02a8736b8f8f8283bcbec51400406971d2dc4e5873f6a430bd4f3e90280f5eaf6a0b" },
                { "zh-CN", "a2b3fa2072156f24bd3825c00cdaa59df619c41ef54bb0f6135bf0eaf02b7c304bf8ece4802187cf84ad3b735f55954da1d8e271bd3f05cca44188d2dad4571d" },
                { "zh-TW", "cd7d8aaa9ec59bb20f221189f44863fb16b09361b8c9b11241f5c1b34a16ef650deb38d30d10aee9a8fa727d3c1ea87a8cf5658ef0dea1dbb6f29ac32002f094" }
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
