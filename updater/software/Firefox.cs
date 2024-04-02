﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/124.0.2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6a2c7145f595205ab5d023ac947b1406386dd4b338f49550f7c1412a36288b2a61e6a4cfe8483183e41651d198f6fe1ed2bcd3d685df9fdf1e1e970c25700e80" },
                { "af", "69adb3fdd5d9fe8432d5e28ad3ab6e81d5e78bb4881af93d02c5e9036b9e9c871cb75fe5f1376dc5df7c90f049f12e460d515bfd87bc8c599357c3ad938415dc" },
                { "an", "7982374f8c4525bd2f92aed155ae32ec400bef220faf6cf28566994fb41fb43b11e1f3fbee47d2b515126e3e87b27936a99be5d7560c5e282a8cd61631445a4e" },
                { "ar", "3b416fa935562ec57eb6b9c392a2f34ea315c4a70674e40f1950efe7f1eba1780224da8cf6cd85e0f7635b42aeb80f24a1fda407cfe2661c03e2543b765aa527" },
                { "ast", "c378bef4577f10598112f9de5c92ecb1fb15f3842d7f7e2af270c53f02a19c3f5ddf70aeb051f1ec418babba89153b0e6ad1e27c4d0db6b68d125bdeba4e81d4" },
                { "az", "cd2b60a2e514df221006419e4c403495eb34d4d89774510a98d988302b2126560b61eaaf583960e1c8f7fc6e6f143a409c09aa6546c140a7c9f30d1d528bfe64" },
                { "be", "d5a20621b60c5b06001a30296a0d19355083d70cfbdd7981edc31e072861f9728df2a977b0e7c784ca7d58ac526647a5a8635163120a44b88f981a34e8eca90d" },
                { "bg", "e15265dcefd2dae1295e76f0f2c45d08ebe52658b03b1060984010174b562fb481b1cebca30919d9306bdbfbef49f90b0b3eb34ffcde3925fb76da13b6d4a1b0" },
                { "bn", "8ed277796fda9d579c4be58721cf256c9f1003303083c5155552bca2c51a526c72224056fd6680f1865a594677cd0497068e3e47f1a6aad4817a33ceb658754a" },
                { "br", "4c21f68dd8f9c48831c37d0434da5ff6167a6a63e28b49798b301a2eb72bc58f482f1c3eb6e8bc700c74514d18dbea7f068fecef499ae12291ca0c81345fe71a" },
                { "bs", "4ff4c1e3ffffbd6b47f0624f57167c107bef6b6bc34c127680a27901d6f75d37ce84ee7c1796ae67673e08ba165dc91b5d2db1d389a6393f703b8af1bbca2ab1" },
                { "ca", "4cf3b12326e3bb699c56d704db9b15aa8e22c76c9445ec7c355e5bd0ebb498f67a3fb9f00e9592dbe26d0cd24b0108b87b6f16a839e290f6e62bd7f49e0b67a6" },
                { "cak", "53a524ccb728fb4bee7d092ef2ae89427faab5e230c8a3bfc494728c39668a542944e51a269dfbd5a2baabaaf5bcb562bbb24ebe6134f7868f52d51a884f9191" },
                { "cs", "e74767a6d710b2b50f9e9451a2a80dbd45f2358501dacb36c96b8d6df370f1274f3e077a23f1279c9a7f618438de35c84864d6a67a86a1c1044c57b909b9e6ba" },
                { "cy", "fcc2841bd8fd491f51c4d6533c51e4d7bdc01c8a0d1d6945725291f661b1a0c42eece56bbd01b564db5490513b8468387890974a9fb8cd38ef5c518a947b6426" },
                { "da", "c14c016af2dae9af9501fe196d9811c75af1ae80a7b2997d21275b1a1f2c6edbd4b33327550c2cd2adf0c28576714d6fd99bf2d708d04ec2825dac024901c711" },
                { "de", "3f8fffd78b8bd5b796bb324881ec653fbc514fcf2bdac5bc176b5ec20a40e9244e769a2b17fc5ea9f364ba4913c5298e9e6810215f57ea6c71a5fced8d75cd7a" },
                { "dsb", "091284f73ccc67c7acf9cba5fd7b2b67b740a7ea34074c32ad6e961a5c7aa8401f6dad62f4c54274bf4a5072b1def9a07013a2efd08cf5fad31a46ffaaf18417" },
                { "el", "c45f75de6fc43e599cd27b6da72caa95aca1de2f8455633d9340da2ce6661dbf9274f569df5030ad51a0ea93ad3c3c0e6da4854f22be4f4499bdcf0f43d266d3" },
                { "en-CA", "11011c8b5428043d7cbdf4e5e99525b99980a10fa073ccd652342fe8d6d91756abd5bdd6783064d5fe0e800d39cc8e7d159e0ba65c182e6699fe65c58bf02073" },
                { "en-GB", "bd7132a680a29815073d6c12797694527b5c233ea45974d2c411dac9006db3a59d7cd8be35b95b73d7e55d91978962c897b1020ecfe291059d5d95c1688ae941" },
                { "en-US", "bce358d57a36bc1d9326f944b7aa3b3f59c3174b8a5d4c7e2ee7b4fe90b1ac3cfb49e79ffb68564359680f6920cf32ac889252aff2a13424bc252d412504f40e" },
                { "eo", "1094f9a4d9430108c79e0005c99b8edc8ec7ec511a01b27287b1bf3e9cb1c6d988f21ba2e078f31ef04c6482851852531d14b3585e52ebdf11167ef9b68864b8" },
                { "es-AR", "73dec85034197853f17cc0b0eed18f9c4cba207fc097c3414f3ebb41cd538b02b258bcb570df827cd6e19248611cb3c26b18a63513237fb167539e754ae9ab9f" },
                { "es-CL", "a0b407414b191a59c1f15a854f672d39ce6f460cf7539a7844b2ddacf6762e40a64c925410ee59b4c92d1709429e9c15aa6d225aa96c564f9b10fe25a15a1c1b" },
                { "es-ES", "da37eef1dbc05b655cd8a8dab5f43aeb8e28db8acec744d8472981b1c520e6b2da3f585675ebf7b73569f8b5e104dcd0db11980be2bdb0f049aa886c0820abc7" },
                { "es-MX", "1a36cce4cb68dde0f20882f9bdb298b2a877fd39a3f8c0caec6a06883391563b34529e4674e3c80d84da663fa9e63bdc06b8dcf15d84a9463e8922bbe4fe2aaf" },
                { "et", "2c4ea7f43bc3751a01d748403cd29705dc89f94c441f29df49387a4747aa1c8b3cf08d0a046d0227e030afac14eaa4c13c7664660e6eede7db9b229fba291a2a" },
                { "eu", "32e87bbba11eb2d0f9a9ab79c5185e16cbb629066c0a287840bc56c095dce755caa8f9be9b169da4ba524d5c4a0f6b26bfea4d78b1d60f10a3a63447afb0459c" },
                { "fa", "9e96163883ac7d52d3a8487b810bae536f466ea50e20de9c8dfa260924d343de31cf4eed2646b4316d14c3b6220f5d7e89afdb96fdd963a60d614f2a57a46e21" },
                { "ff", "abd1a45ceec5a77bd63fc790507654be919ae8969b14528249a3fb727717887abb906bc6720d13eb8e2189f26cdf401b4492ea9d011fb17f445aaf59952a0b28" },
                { "fi", "17b41e576ceec296d1190788c08a2d7bda397d6e492053f9152dba9f88c512ee10854a005ad954ee6b9d1d56f78c336fa6bc2de357c36ea5b12e178ec3bb41aa" },
                { "fr", "f0e18e129b35aaae9bfd7043d2980c2e98d4d9889ff6ddc283cd6166541aaad58d34e037b9cdc6d0c1cfb66bb6e653bc654f8820f2879973a4f2227df74091b6" },
                { "fur", "d6a111b12effa4c61daab39b703f82f8398bf3ffa447a4096e6e67b3d727fa354d52657b99a16c90ec163f90aa743c170987d6b0d2d5e224f8c423c164b7b3c7" },
                { "fy-NL", "31557ed5bd0aa53ea7dd0c9d3fb0d24a2d6a5fc0102c5a2c23fb88daaaae458eeaa30e2e1fc9f200c2886d9452a31d7a0b438d7307cb5fcc2dceeda49d531724" },
                { "ga-IE", "6e9fa26bcb93f6659b7100276a1c796c70952323c2007dcbd2f3235b1432e426c62a5f27f112793121fa5a9532850f89ae396c6c97e5c5ccf9cfa488919c4389" },
                { "gd", "8c6ca5e6c929112c81537e2109a5119e7339c0563e3dbe1b5fd1f8791262216cf824dab0ea6243c75f8085c3e84d96bf4e325484e71c15e831b8293579d165af" },
                { "gl", "d69fc44f77f6f745e560a8da1531bb7be16f7613bb8a5972d0f53700c79834d207fe230542de1f9f83b1789271c8728b5a7c5fcd9e44950ac5fd72cf42e8a225" },
                { "gn", "54b63497bb29dfdc80f7e1e0431e37c15dab19a57bb8eb2d73bae2c5a863b45491f96d29e848b26b1c880c3b839db20a28394bfc881e10bcabc19cab8daa92b5" },
                { "gu-IN", "5c8d74dde5f488b2213c660f4265db2fc55c465858bfc7b5f51cacced9ba561060ca074bf92f6100860fda6e174523b35f2efdbdacfd58fb1f81913d46e040b6" },
                { "he", "1a9ee463cd9f068c6ec1df82b00168dbb5c608f7bdec54a169fcc7729512abe4864db5c8a5e0d590ccd5f55e0389e1a61106017faa921a66c56d524b0e87d6e9" },
                { "hi-IN", "7f4df816946e6ac213c111a300c199494b7d246babde78750ceb6b1442f0b2c7a18036aa7984cbbfc51d1bfa2a190ded6d5a55ba8a6030745a6041038085706d" },
                { "hr", "400087a84a33c0b671990f8e2d6b293b591dcd6e8ad7dfc1ef8d57c77e87a6fcaf90e602a52519cd1529cc145f6908c11c5e1b818d8d7c67a1691c8c44560286" },
                { "hsb", "00556a00e9d5d2d3e566fffef4322deff1a540f44388f1dbc81b195b65e7e9b500543e4b132145c5cab90316d0f23fe04c7e95e6a8360c4531bd4bdeaccd074d" },
                { "hu", "c783e026188380f9f12032ab4f2667642eb231c02cfdbd4c25e32ecd4c2f001d5a84ae57501c8aa32bd8569f5c8137c74a9eec944fcd9f7cd70c4a22a76e7438" },
                { "hy-AM", "7fde38db7baf5ea8b599ea0e9caff687d4812b32697b6cdd6e9aa0abd24db36a6844a606d8b8f58cb4ef1c726361a18389eea0bfd276a9a590145904d7993a1c" },
                { "ia", "8394702916332bff567c1253546fba98208d8e7275c47163c1c6ffb7f57ad1a74f4578c1e5f18e541699b375a69fa003e9f6a4d465277ce2aadafd8801a02dd5" },
                { "id", "8a073614f4ff995e8ad8254597c404c3d0f8b3825d237c3b7da22feed34e4f87b52525bfbca6fc9ec920f6f539832a9b502097dc6ce3e74dee3369c8de4e3b94" },
                { "is", "428364e192aa3d9add89d23a7d3fa0c959399c5fef1ea1fd0f746b9101570fd7e4d8df3a59158a72be278b2df7a136cfdeee7e5139fc608bd8a01636012ab302" },
                { "it", "5b26e7be1c67f1f8ce4fe05c6888cf81380000e4d39c70307f7a02020e018db93d0551787da89fd7347100b568d30b87d4a2214d969e72b6f63391acb5e57b22" },
                { "ja", "a0d5275dd34aaff27bff4462ea6b5e70772e12c3eb52ef0453179a27fcfc3fbe6237253b06b7929afc2e13133667ac3d11e942634fcf26ce68ad086e0646ec6e" },
                { "ka", "49e3f39a7ca98066e0c969408c96aa3c7a6deb356ee896d3df0d0e1a15b9b26d20d894210bdf0232f488707fa7079cc887f132d4fc5aff4f0e5860997f2d850c" },
                { "kab", "4367192aa16b50bf1eedf1ca5d689c33183450d3a18141462f048be69117616d96dbca57d66c17f82bb6effe0b1af15cc5b8a00d5b106749556c52b4b8260756" },
                { "kk", "88721fe45b3af4a85ef3ef646cdfddd74688d1e6dcab5d5b92b88b1c9ebfaae19b156b34fa7211430853cc9194a2aca774dce7c0f6fb1feea61df2f89a0af3e4" },
                { "km", "f8df06cb102b82fb59f77e9ebd655afe27dcf74e7b863fbf82a5aca6960cddf02dd4f83544f028ceccff0f0e1429c353f82feccc4d73b14ffda7e7079b4b0068" },
                { "kn", "82ccaa0039f4045cd1d833335b3cf17ca38d2f2ad26b4151a02ed5a14d6918c25fd1d1db99ea05763f9150f8cf9c1d7075e3e51971bde4d4605371dd74acba7c" },
                { "ko", "a10935192ea5226c60f6ebccec43753e97f66bbe67edd1a67ce8b9b018629ba91da37fbf2d6b421bc900cc85bbb5fb3b0a2c0cdb02c5c09cc784b213446d7809" },
                { "lij", "1991df5d502e78dffaaf589d21f6c182c188430b941b8ab03dd4deef180ff3e15c2ce925c98d7d2e3ef27f406b3b02fa054074770a7db958d881d069eb7cbb7c" },
                { "lt", "1f29213ccbd8d875417c94a2c1d65b5b73b5bea0b7f4d6e135c2d792985743fde9756693901e5d1d556952a1fbcc2cbeabe3bee3904f4069b3ef2aa2564c2a4f" },
                { "lv", "ada115ba6e916a1e3eb90cf77a595bb9514e1504cb14f8340ae9c30940966b6496f14794ff143fa2500e8474a47a697122ac4e8631b377bb646e11c76964f8c0" },
                { "mk", "6c5648ddce5dbc5468c154309e717e6abc990f1b10e53386e9872644ce68a889b891d58b8f005f1d368d4c012ebe9b5d7f9cb3939b26c9571092db8001f9d8e8" },
                { "mr", "bfc2845c542584d378bb89813146ddd80ccc953bbc02ff1de6712768ddecb43e8aed3ac68f242a336ff35a0e3520ab9604e89d4011a70d760d304f7efb67c708" },
                { "ms", "36cb5cfdb4237ccf09007eb50a9cff9e6d4945f3ee69a031fa384071cad8fce4b4e11d3cb638e32c64d92540062a03c70a74aba1e359c1f4ce55aa65a71d6114" },
                { "my", "a5ad64b305507a922060520f86bfdf840df4cb366b775a579df97e0f45ee341b51bcbf421d4847840e84f9ebf1a2ca7f16b006764f9fee0a05609e06243d96e7" },
                { "nb-NO", "5cb671a4ee672c4c68d757050263340b85fec20408ba5b4f2a57baad95320633a3a295a7c929cf4b26595955f04ce56e8a278bfff02aff6f93fe519829cc2bc2" },
                { "ne-NP", "cfd44f743711107312e6f1cf947bcff110b3acd8db3a64c076a3a82dc1de53ebb308f87abb96dc9e85dbc11de0bb74a44b528e0fa3b8636c4961cbc171ee2fe2" },
                { "nl", "12f968f8895c26528441cf6f0a9ce3465fd39013830d62c47679e914de9b78196c90ed6672a65972db00ebc939de9030765b52b48e7ebc84a9a06a40fbde7935" },
                { "nn-NO", "5b0365b98449183693cd20966b4ace3f27bc0d914bfffc71d3682444e2c416de068b02f863e0f6425d220dd1b4f1f52c374a2b63fb647ac87184dc02d9a41dce" },
                { "oc", "a525b6ea9563da06caf9d4ad8659de41ce3ef6b2df33b89ba33023a5606c40cf4ae9f7012e129c9599019322862f9c404b7234f2b1553179970c2279b4d127b1" },
                { "pa-IN", "e556f86996115dd5816d18180eec4e897bb98123205fd2bc7386ffaf4aaf33bb4f8ae88f1c189c8a079ac94dcd28b3605177d053cf13cac82d2f29617321a814" },
                { "pl", "4b6c3da3e0dc413f11fd56d844c2fd979d9ffbe59483db20b35615f6ac5490015604cb82b9b75c16d53a847fbedb9fc80e625cdf7dda9dec6777d42795adbde5" },
                { "pt-BR", "eea40f905eb916536bc5a7d428ce99a5da6bcc6b1a670097a140a43ec490bb6116db89da05fb28212211633b234e1241df88c522388ec20354ea9e8444108085" },
                { "pt-PT", "ed76b7705ec1984a40ae48d4c30a92e4b8a3d5600c95af5d5f6749bb2c1453f406904afa8b57cbb454c6923762fe21c710897325de8dbf74ecd1aab135b6f5f0" },
                { "rm", "e3dcc4de16cda073cbc2dc48f586e5840191e817c122d4878ddd0e962a494fe7a0bfa6cae09289b610780ce9f617d0357189fad3748fe76e01b8c1f55c539014" },
                { "ro", "51ca2983219ec12b65f62dde2a4e8eca9b824de439d00813d2b5f029a936874cc02697fb876997a9624e6524d4445d40f9532dcd9a75ae1512edae9ef2d4619a" },
                { "ru", "424a5b40ae1c382e08e5ef3686ccf44609f50c29740c3bd5ed5ab1bffc7fedb98313300cea37278a31ed488ffc9b0a2c0918751f7c5947224da9820bf19bcf1d" },
                { "sat", "f118ff4ea66e779d01b90679b1fefb611d5913da5ca77cc28cb9213644ac142032a634d517a2c48cc479a3ddab5ff788e8a9a09eb4753f0477555409f5f1cc26" },
                { "sc", "71e88f197cea7042167af94a425dea37de71a2d6fead4c0c08fc554e01370ab5b29c1d5e4769aed7d528c8006d5ba84ab05b7d958c680fa33b5b25bc88bb0cba" },
                { "sco", "789885ba1bfb6b668d027ef33f39c33fa6ae7ee94d689a7cb090b297863830953b275b679b1cf1876d615c6213f1e26dab60bd768fdc752da0774c076b914e8d" },
                { "si", "1f77a1932ca9994f234e1ff3547f8fdaf607472fc83dae323603e3d97674504e17c45dacd197af4f42469dfd4913ef61a621430695d4350c003ddca76d203e68" },
                { "sk", "b97044f3c7dba074c76b46221357c3d3409580c53fddc01350b2c62539bfec7c1077b55538d00c35058e616d470d8b3dd530b915e1cc094c0c9549b9b8950158" },
                { "sl", "a065cb353dcd71a13888469b16da59b1a9d361738fe348beb43d4d7c49de94fb3d75be5c70f7c440e26405465e5b0b9cb0458342bf0fbbe41afd047eee659745" },
                { "son", "44444c4f9d00c26789b6cee2ad675b33b8e21f7b11f498f7d72fa3dd94b1b2b0d1ea558a70b18529234f0ed995f62cfd9aab12a221da77dadcb6adc9ab6006a0" },
                { "sq", "39775359d74008e7b59591aff973c83ea90ddef5e2fd303cbabde11a5a0c24329309aef3de78107b4c7f7875acd1b91e5db55f2b8c05de950779d6d70dec47b2" },
                { "sr", "823f38199731aab2f69067fb61cfb9d151d78d6afef6861c995030fe37301f8f4e5b405cf1a8c342ac7e22863cc6633165df3426b915aa70a18e3049efc7cf82" },
                { "sv-SE", "d0bad0e9336c0d5d5775a2af0762076c0b7049d5c35bd9abd7e0ac31a9fa9ea2a0bd6bd817208bd8170807530fab11d48f44ffb7b5784ab4d005eb8324a561ff" },
                { "szl", "d06c616147dd82ed0287a6b68f7e48b5a38453a9b55b34ed6427304855264701e04fbfab1abb51f55d2a30ae29fff3cc8575d9ba94baa2278023c867465c3543" },
                { "ta", "fab15f56a5958b653ffe3d6aa61d3c5ff895319de9c245496272ffc32543875c4c98a48aba26d76aa0682fdc29b1cfa3e395b296e7f878c50772e442a33538ac" },
                { "te", "490f9f584d487504422c874fc29d33ce7a07d0350ff5ea7b3b07aba3331df7a7a0dc21642dcb2e0cee847c0e38c0ccc174284f2e2892df0d29e04a631df6e2c9" },
                { "tg", "cbf81fdb0a2223f8936b1c2f705ca3a7f165244f2ac39b055c873484c1810a5458e404b68550b81873491fdeefdbafdb6c86b207dd760ebbd954581f0c50a675" },
                { "th", "44461f8af406e5a28b3b72d46bae0e7740bab0e563dcbf88512633f7593e6ef2a688b85bd7f963b9e9392ec94373a43fb5e9140d61e9892c0afa9093166be78f" },
                { "tl", "71ea5319ed85a5149020619790e3bada1abc1e966b4540e267cb4454dfb1347e342d54f7db5a0baf1379dd41c02bde8fbd6a340a991ddda97080b87d8ef61f0c" },
                { "tr", "a7584884cced4890c054434f88c6b30af86ba013d3b421f3b11c8a39e9d948f0cd96b0ca767983075fbd187eb1cdd5d30640aa73bdcb27a606d1f73bbfc50503" },
                { "trs", "dc3cd151625eb90c84ca1da8742f0ec99d0eb0d46e701e8637dc477ae348cfce5c1fbf24bf49ccda072874a4539194506ca646f8174424223b48c66422b9ea8f" },
                { "uk", "041e252c56d283f912da239d018eaf88f4fd2fb7202a498f188fa30cd3dfb08df9ad6835ca46e85b1d25684a536ffc30e7025d9b166ccb2a9445e4735cab2312" },
                { "ur", "26168452ae2c6800fc75708832baa678acb402acaf9b52978959b72d2d1985ad4fed429f79b4351bab3f0a5ba0d9539b76ff4c3b9694572ee600dd324ac243fc" },
                { "uz", "11cc69e938f16f666db4c08f4b3bc27ff288c9ee90a78c4c32a4d2882a8f8b5a73c9c502466f39d8dbe9737df8fd7e4ccce841ec92f6a6518373ae908d60c63d" },
                { "vi", "e6fd9fa83be00c38b2644583fa6163dd5e459300138b4c7a43c7dec3c6b7aebde781826e5931c3255b098c49315fbb827dd029025864f52f9bea01e2a343ba02" },
                { "xh", "5bfcd171f21b78f86b546867f44e90ec5577f772e3c695d6ce57f1f3cb14740de4f0f2b037851c795e79cc6d5132deacd23bdf44e42564889410c6aef4bf0573" },
                { "zh-CN", "47d1db39a84b166175c4359d6252effc0cb030335a122ed91e4ade05c20ee0ef3aba76617cf12f8917531a350721d8bde579004610671818a6137e90e7a871b8" },
                { "zh-TW", "b1651d49399cc545216925b0859d16acd0ea219af88b117084e9c8925e2c3111ddc6b171bdff7b3a557b274ac5503704b9b34de8e3faec57a7aeca2be302317f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/124.0.2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "e14cc3f92bd57abc1b4de2fbadc4b229444019758ea7cdaed2b5dfb14afa5cdf3a32715c83bfa1f6c2520d866b87ed1322f1314bcd7841572c58d9e62ec4149f" },
                { "af", "d556a10e98dc8d5dc2de26f8bcef97ab4c0e6790d2c7732abb2b74b3fb9e2e5caadcf7cdf27905014e2b2026f46bb48f3efa07fac34ce614211a4a8e98c8623d" },
                { "an", "d4a0641534681555ba2f017a0ec7197a1017b50cb549cc62870cfc8d9fed31fa2218ace571766326c3aad3a415c049a90de43965a612e6e2548e43f530ef7128" },
                { "ar", "339f112a0414170d1f8ad6607ae6849af3175cccb250e7728a466abe6fc4498d86b402ce3dcc1c37556e06b79a0d1fa5cb6def432c009dff692973e4230d54fd" },
                { "ast", "4d7e4c94b4306b82f13aecc9bc3cb04132fd5cfa1b8e0fedfba062c81740a048d4e19b3afbada74fbe330f68f5dd0ecb1788e48e28e4123b97392ebdb22026b7" },
                { "az", "babff6f0cf2a4b8d076c354b8c277152354bdd0626ef33672c3f7499482ffe680bc87c15c6aa2fbb03b085b691a5943790ea52431f263d97b4e0722ff43eecab" },
                { "be", "fc3b39b80dd48234c018c800cda2191edfb16cfd24e3406897b1030634764ca48720a5100fbb5ad229aa7cbc7ea88f54b850dbf2a26eecab58048bd15077bc2e" },
                { "bg", "5043049e50dacc53e64929895337463631ad9c880ace8dbd31487c7752bce551de00e1513efcfc3cb6d72ddf51c7dcd9d32995caad4821475bb5d814767612b3" },
                { "bn", "e71a0584075866ae3b13973b1b4e48d7e6647e23603624e7752669e9b536da354c203adf7b5084bc6f287ab4d48781fd1c5f1cbf4f9b86d0c79e8a2bf0f9e88d" },
                { "br", "92e187170df594fe80d8c4e7599630045f2752e5b64b8faaacb67fd3897988507140e2762a28e064802b5eaf1a6e10d729d490bb52f3bf67ae393fa40c88260a" },
                { "bs", "61b24987b4a293f6bc1ebfa62274e591aab87305f878a80889ec778b2624a4c3a1de4fa0292a41e72e06b80ad05e19c1ef01c22b8475c9a14922f0cf7b3eba4f" },
                { "ca", "7c96d1d1d99812f9469aa883595e53d962194dbe0c5ba5ba697227c97ce663280f0e3f124c16eefee588170cbac8367cc01b9003d045b0dd533543a277ee17f2" },
                { "cak", "4f71c4f10beba5e3deb5a40096c91f8d03da4d4fab44a0bbb5cf4343cbbd4d3f2c85ab624b76a004ccd00fb675bc93fa787d812a495d2e2fbb839ce4f6ab5081" },
                { "cs", "e4385e4422892c049ef33c56314e92237ac55570d124030e6f224cbef121445743caeade1f300aab4d50ff50683df1e0decf41f32e5083471b1d0b18598a7d20" },
                { "cy", "440812936629e03db7039baf9d12f23132f76fc06d7d4e13b97a55f00e6a127dfbc62dc0e91fa15a0ca32f7d67d39bfd89ef5bee49e427ee408e05a9ebe70853" },
                { "da", "89f5e92f38f76dfe79ddb9b87cdcfc144c765a91c4f89730c30c35885cdb057b27e9c4ff9072b920c71beff5f6ce22e49d0efe0c4afccdbf5b9c91596a41805d" },
                { "de", "b270ffa94b4b0d5dc19e62bcac374a134fba0bf2b6b0b074b9f4cd08d8bf1d9eb928249fb5f4f8feb87ff08bbb02cb62fdb52d8e4a077df0d75bdca92a41740b" },
                { "dsb", "4e26f1226da068213ed693f01a64a1ba1fc33c25882d08c96fcc70bcd19755f27df11a680765dbf25e3a7b84253b678d77af0263719d99d1c6a5f0fabe2e42e1" },
                { "el", "3a36c58e155281886dcc7449dff48992e5da1fb3fbc8c907ffbdcaf320195c72ceda61a93359812e2cbf69d2dc0a306d1ea6cf8a785c100b62f7003deafd69a6" },
                { "en-CA", "bb8c8aff6a7e9b75ec8ee87286697ef1729140d4de5951a3fa94c9c8bd84e4e5fce854d389aa059c1e5433b8e3da7eaf9ea2a2f424b7dd2d534a9a806cde8c7f" },
                { "en-GB", "40e1de80776a18626c48f4bc4a9e4236fa7acdbf9029a635d60e320afb751814d695556d13f69bec3bd0f42831329da3e25f84f866492a9cc8803b596cd3854e" },
                { "en-US", "fc7a5940a0a32ac9fc45771f57e709c3180f3985d59b639b330d458cbccf829b03c3fdeb0015f43ce52605002498a76dbef2e97001b113d6651e779d653f9ea5" },
                { "eo", "075acdc7eca981bb7b45ead3c1ac6dc498ee1b4a3ab92e3790c8f90526faea4f5e97bbf37735a4cb75221aea7a887e287cc2ace1716284e88c7f3af0826854f2" },
                { "es-AR", "5e6901e3bb733697eb048309da22ec0779e7618a65ac93ef229c36a5329fe19232b5cadbf473cab7d0dcc639c43e6686328b0bb008eb25c8bbae15586c8f6d28" },
                { "es-CL", "8cfe47bfc3597d63f1c79050d1aea385d58efbab191979f642eccd6b4659a6b043e9a3044d7e5ba73043d603aabf733068445e774e47ff27e7652ffa964b803a" },
                { "es-ES", "02b238666f488159df8b19b9e61dbbdc6646a814d0c82168762023af92b0465c6819e3cbae33d846dd58a5fb80b5b5fc503c9e9d2b19ac26d18ce8e23df037ff" },
                { "es-MX", "67383a76279e5e9831c065ea0655bf1ed4789a9c2919ac67665b327980cfbadcda97b83587ef2e5ed2fd02a2b602ebeefc1334d5fd312ff00bdbca8d6d6b420b" },
                { "et", "5936d3adf4d4d68acf47c9f51614a0350e09a764d523290bc1964d6b3caa6f3273a066658ca653d3c58a9c3c71dbcdeadffe3ff2db01530f6f298c383918220c" },
                { "eu", "4bc932e00a5bf677c2e634a09cb65451e827a2aa8ee85126112361176c3dc4d48dda1d626efde1f84c106270c51998bbc058dca8d25b0599f5305ed377735edf" },
                { "fa", "8548d2f81f8ab61c6bf20035a36a1574eb1aff1221fbe3eef61ff84fa8748344ba333ffefdaede972b23f176cffe0833b8b942e25332bfda45e8480122047fbf" },
                { "ff", "311a1ca8da11f803c8bb993705a95563c0e2056874fddb7727b19f100acd5f18789b78c0c89994e00b9699c2adea277b8ff9b84a16d60b9f09873c2ba992dd1c" },
                { "fi", "01af32b930e020e7bd5c74e34350726c3331fb22e732d6ce814e5c59c93aa70afe24ac1d4c0a10fcdea7b5d84869afd73ca0d235895cb622ecbe448d10657e89" },
                { "fr", "9d862510aa436aa728198e371fd84c89944aa363e037fc427dec3b55b164dd6d0b325aa9954414e339480f2e48dfd27e6eb684eb09b75a23203b22a5d3e22c54" },
                { "fur", "462d043bd6a5992e09400dc5bb469a080a13bf400995355b8e34ce1ee01c15db63b98abebcc66acbbe89735533edeea3e2e5cee35fcb4f54e2db516975102ad4" },
                { "fy-NL", "706db05140bfb71346b6e2d31386b788d11a272cc5fb5fc0a89cd02205cff674bec46e6302cd8d17ecc981a71558d3856e7672b0b920659aca5608973b9cf48c" },
                { "ga-IE", "16e2bcb097dd36bb0357bcc6aa43cd7612ca7ab31b742e09a3db29d95a4b5eb7196c92d94b531812a697b62422980af5306e2fecfbcec3a32aa118c82961aa1a" },
                { "gd", "64f5867583b4315d207453f2e121206ca1d085ffd22bc3f673a86a6deb9d90259e6f4701e40bd7380f641b5e913e93ce6d4bdc7ae26929295b8d78df0fc18122" },
                { "gl", "0b1919ccc6c2d40ba4d3f149ed6c06cfc447fe55dfb8ee19152db1a92f78538f64bb39504ea1c346f75adb780ea9c8b771010fb873cc3e11c57907a3f4be4300" },
                { "gn", "ba070738f8bc52f8bc86e867aa1d610964b18b1fd55ad37e51aaa02b29f04cc8e5e3612b85cefcbe3998125f054c84618a43f2391b5558312d2f0bf7d6b9153b" },
                { "gu-IN", "55829b424e38a1a1f21fd35a2ace727639fcad23be43167b9991daf1c7012f04ba803b4dff0cabe631709e08b97b8214d646e62ee60a23f11a2667f924581eb4" },
                { "he", "73af6da90c6010bb1070b9baae0a304724dedcd4be10dabb389545d3217b62084a447ff75d7eab3dfd7f8888f44f7c58b9a9ceef3ed562b11da6ee5e8992fd8f" },
                { "hi-IN", "3f631fa7a3d9312aba267dda7713034e3123a142fe48569569b76b2899ff3917391ecf72bd855c3f1891ac5e6585897f47653d88e8941ff2a096e8bbaf8e5218" },
                { "hr", "ee14d3d90dc8e1b6e0dac6b8e4cfa92c3d282235d345af8373d9fe6067494d135151387d0a1a1b4564453696cf67e02fc72f2a96df49cfd47fc802e626c11ff0" },
                { "hsb", "633603bcb2d03c074d65b8844f434b8a65b30df181e2bb17175b20b2a791e3af3deba58c2595ba5d23f422009037cf48810675ee815acd367abd6bd1d8fb6502" },
                { "hu", "8d64a6c0b03aabc686f0d166d1a513bef5e2f1de9954eb8a07ce6f9a16977a6f79b6ab50eda8ae94d01735b61a82b03d8031516827d3bf9482397e7e74f1624f" },
                { "hy-AM", "577656eef4bb828f6b7efefd4ed131f3499e5f5af20a255d49c241c159888f1aedb709596cd81017354e18aaf22dc4535a0e187673e6cc6e7f19e71ede870d27" },
                { "ia", "9a55cfe33d3801ff3ed9c85567c531b2e2145f05fde9ceb7b25771fb5cc3508b15dfb07d6afc891aeb3ef63d988e5081a3181806ac646894a65b5ced4457371e" },
                { "id", "f42f2d9790758851978ddbd21b0c2844a5b3c02f413d3c14796c3b7dd27583ecfc53ccc42e6a5883d77036486014aa0dd4313b81c0c30f24ce0f11bb3b8f075d" },
                { "is", "ff3632e5144026ea8c1fbf87afe16d58fe8f02540992b55b0bdbdfade39a1db80ee57817730cf85500ec3a198860bf0c0c619b0184d28d79960e09d20632a1d8" },
                { "it", "fb6607ccd06a005489cdd2fa44ef98eb626685899f00de84c37abfec55e017f6d07a70e60b36ad869d60e00851feb1e52bcb17787912d0db2c47e754aa4cf6a3" },
                { "ja", "574b92a1a0404772a737802124b5ff6d8d6c4593f2629f2ae03dabe83ea39cfabdddd425cb134f8f1542bb21fea76eb1e85ad7c4e66fa4ac743845dfd23e38f0" },
                { "ka", "11247ba065db56a3d3d139d61222a2179e9428561ed8b1b802aab90e43bdb61610ecb1bec9b133e6c78716c1487da8e04f9fe1858e2f25ed0be23b975e2c0b7d" },
                { "kab", "ac46b355c673da3d808157ec923645339b0496af5566f8792bcc8754d785580f7fe07590fcf633ffd270fdde8c05cd4b04c5b1547b2256ba92fb5995f95622fe" },
                { "kk", "9e162a73fdaab60c143a725f0ecf98bb535c98b9119c5e99efef280d31445118983d879c384fc6c293152aaed564abef3efdf460c4eb601c753cdd2f050783da" },
                { "km", "0d5675f0571e28c6bf19e1ed094b92c58aa98245a532aca343635d0729a1eaf47313e3a60be8100d5d4ef80454d3efa66d945ae4a9f25d4308ccdab5bf91dc47" },
                { "kn", "7d873a1de7f74f2c157f9022b50a30c9fee93054a85cae9b7066c3978d90b692cd459205b423ad1d8b0e514012c000f5beb1a5bb8b90ae2a96e9c1b43638fc34" },
                { "ko", "ee588634959be18edecbae91ec261fe99ef2ff731cc0d7868f9f75cf129120998f5bdf537faf740113d06ac656991b3298c8d3e7a2dae1a362d212d7fb1ecdcc" },
                { "lij", "d91c50784b44bfef17166d53428823bb9c9c6b566b0ad497da844c0f79cd631dff39148e85010ccd88c1c73ed7ba590c113303cc4fc89bffe4e33c8fbd3f9b6c" },
                { "lt", "bfe78dd1e5011270b81323e51c7305828d793661e1cec0dac8bcd1a054a53e7104db7e2bf52a43b4201e04922e75fc3d9b9535e65b2bc6da9a92d7c6f384b2b0" },
                { "lv", "462c3f4ccb74123fee9d3acdf16a57288c4c21e3eaa5158713585142ffdfe72e45a43f6f1d8adacd22ccf979618b05d30fd171c77548b3eda54127eda28daf1e" },
                { "mk", "b9794acfb7d59bf07234e29235c20a050d66d5303f446d5b6ff3e70e2978899b5b16aa90e8b50487a80d5373fde0104e094cba3d9dabe12455c658af9165febb" },
                { "mr", "8fa100a5832ceeea358aae4399479300c163e8dd894e07beb9f50a36aa872eac7a9374865f17d0f519cdfef6f665a539c127a8172b211c44ec23c8cef72bcbfb" },
                { "ms", "5428ec617feccd8a1b76a3b035830f380028cd23b5dda5ee79341b2d863a772302d521bb81ebb87265aed80f22b75c589a5db79bdbd37f2d7f97da48575da771" },
                { "my", "f288f1876e032435b44a99eba8ee1dd3c770ed8eb4ebd9b11405a1126e37f41e032f42ba54d5c75d12464d968eb436a7311dd1f287b7a155731a1422dc975322" },
                { "nb-NO", "1ec4a4075cd69f73698d275a7d9b64f0ce197aa3546ca8f39de182433c3b2f5fa4327757562ae3f898773775ef767b655774cb589c5df2a2d630091246507cd6" },
                { "ne-NP", "cbb1f085b25e3e8f0b6abfdcc7090b48842d21477d35812f25e1e3d546afa582b5b1d2824cf8c7f8ac3c99e2847faff3e3076e246323ea380744bcad10691736" },
                { "nl", "96fd9ebac5f31b459342f8f18e240780b1c1d5b12948c410528a5498f33e0e885dd233ab7182527bf8da4533723837d7b3c94f801435078227a3e2d0a4f1f527" },
                { "nn-NO", "02c5091072c3aeca41aba30e3369371d0b8c0e421783c1bfd99434ea145289ead99f8358b25195989d0755528475523b710c1760439fb07d4e82078d2928a7a6" },
                { "oc", "9a566bf5df5c3d48605ffff9315d0ab7b35ac895f8266579154b007d70c5a048ff2da45601f8271912e3542de64755e3d2f872daa8746dd6cd7dfb22134ea334" },
                { "pa-IN", "3665e1bd59f6026800fc9760c3482aa921ab0a1a5e65ac6cbb43d1509e63c46b9e49b808d0ce5ba791ae191b93b925322ac4916c135c641aaf4ff9189ca5634f" },
                { "pl", "881d44aa5e3009ded0c93159202a225e471a946bc3f08af70b7f1333fc3f927fefff88a8941888e2df308ca06899b170a29691585c57d51f438ea8ecb6d74b89" },
                { "pt-BR", "a853495c458abc8a76977591bf4e94f54279011c10efb09cc9d29089183f683eca5a29d2eea969d1fdef3f9d2f852994f57a6631616ab588cf3f304e14a1d121" },
                { "pt-PT", "a64fbc7f61b37c47844a6d38576fd49b9c1d807197229d4ce4877161b4b99efb1babfb2b1369aff14fe2288e0c2c037166a829026b17d0f2e3e799d779cc3a95" },
                { "rm", "fa2dcbcc268119bd87dcc37d176859fcfe10c572c11e91d6f7060791c691bcab6a670b6d177e7460679d0fc09caada1310984199cb8db9bdb6a10548e14d3d09" },
                { "ro", "d2c6498a456b7bf594a7c062f94f869269dc9b5e4e5173e36a82db9eb683b34bbb4cc7fd0f10044c9c0e3aaa67918003aa8d4956c817d3df8d06cef6427654cd" },
                { "ru", "bd61f9421aac134f9276385c7b30e8b41110898a694f380f2a629d99e55b6ae075aff65b570c38514be65e7a6286fdfa806a60b76fff9b58771ed9d18f649062" },
                { "sat", "64400b70c07e2786b17b9518d6613076431ade63f1a5ec5e63927d3e5afc876ae4d91ffe71614de7f63c9ffb633c45354cacc184784127b6716a8ecc12fcd254" },
                { "sc", "0f0bcb6d32f5ce83273d4b65e3c5ce975ad40522f612ac0a4b912354e2d76933a46aa758b74ca88aef0fb41c3ec661f7cbf102ee3bd0c1a13ed0d46ee94787ec" },
                { "sco", "a75c2dca2ddf75cc72f91fa7a37896118644d9921272b8173c1e298ac7014b15e3e30258e05ab92fc5d659e29347394d4be02f6990762dc77165070a7a3ce1f7" },
                { "si", "33727497eee4bde701b614ebb2022136003abf8e33925844faf7ad64869dc4ef5ac39806c7394465742325eb501ca5168865ab7f40838227869f658c321f1bfa" },
                { "sk", "5b190d9c81f875c8f2168f484183e905e0cbfab0b748d2c467d5ad63a1e14a51592527976283048208ec6bcc7e04a835c9f8ae92201909ed664a2c6e3fed6634" },
                { "sl", "5545cb9761065a0ac861823bc9647b1751360f45fe1d753b17ba2f06a0e087d2ca1bc18e1ab2477a9ef3983d14abd221cb6b9132b347749f6d59d68218b2c2b8" },
                { "son", "295b89c91809b0ffea061375e7bdb5c3687864a05a436a68fe5359921ee4f5bd746e20f083a8ca823920f06aa0b440220a9535b0472b2cd48d9bc4a06018a70b" },
                { "sq", "e1d452e0c5f425e84a1e0b8b15b5bbe0a34e598a8c50c0bdcc8326b24d67f16c52c57e7615f6871bdb0efe5c4788e9106fc40308ebf4eca6828e748b42c0106c" },
                { "sr", "e213876c9d16f0cd5a708978962af2622a7a7453e774d4c80d0997c2e028c1708f3689aff4aa70da2852e5c6b083a8558148d4f6e373ad3148f43f370ea4d170" },
                { "sv-SE", "dcf65365c726fadc4dded132ea02dea94e05498fabea78074220716fa2ed24141d41d14d50c53e01f2bb128b0640f7cec67f799665b87d5d6c2de441715ab467" },
                { "szl", "8a169aadfba055e160ec9a6078c14f2150f4c346e3a6844bf1797f241a876ce99e9f21a7af6724c6035764f96f9f3b3aebd771e58b82a382473f08b757df561f" },
                { "ta", "8d12c3308768341386822088376cfb1bd2e3e9cf5eb5091ae2bfefe22015f1a16881c75e2649d5d7810c5fc1ce70b7898364a1dfb7e5132091cbf75201cd091e" },
                { "te", "b66fac82265941b2415277120a69e3179ad10a5fd227e645b027fd898cf55d8f1b3184639d1034ce9030f90b4a0e4974b24929088500c1009c681dce69c07337" },
                { "tg", "a3f9d5fa331fe2106c44237ab739e72a40ea4576330212db82b6b5091db40f937240a5a48c0447ccb35bd00be2cb64d563f276d15d02ed8cebe05d0d48b6e9c5" },
                { "th", "9615758961efb3b1a1e56f6a1bfba355468fedc349a1b19749577ff8f981023a9ae440ed4acdaa8b57d705c10bd936865b56607753f972528a072197d6c66524" },
                { "tl", "f248ae878b97296e1476c23640031d910c2370a0b204af818ac4c9891766cfd25bbd978d244506e0a9704500979d7aa3afac53bde2733902ca805d2c4fe933b0" },
                { "tr", "8de97ad14cc8aa26f1befc3aba6a6793feeff3b71758e42174b4e7c399fd720d8c3052b3d8378e0b18a648e5424438bb392e6a72f0947575321111e527229008" },
                { "trs", "6ad396a59484ea26b4a02c3f4df763f4074027affc861fae8de2ee87ccd41de3c186698d5b1a5fe8ad423ac05f0888c1a32b85b15b46e2f39a637967a0457c0d" },
                { "uk", "7770561c4fb0900343d45aad2adb1ec372b817f4c7279d4570eb817dae1511f3f09bae4ca664de547337443f9fdab8fa804cc420e6c5d63fe0e826695048efec" },
                { "ur", "6e72ad5cacc9b9e08ef5c85319506d3f8548d1c4cb6ff79e71692413b61965c8150ab025336ee646cd17f3dfa0b7353251b4ea744938f6f50213f3b54246be55" },
                { "uz", "52654ee5770a60ccac2d832602e10a75ebab04b71e16a1a2df7cf39e06b153e419f5e66c4bd189cd6f24fd0753db37425070834a5e55922176f020a6fdb1ece4" },
                { "vi", "d7f36e869962b350798edaeaa740036764e5b993b9c26ac2bc54c65a76e15ca78803710108fa8d20cbbd181fb3532b49fe44d5be1032116f0ebf715a9f2e5202" },
                { "xh", "1c0bb3eb69c8ed1930ebbfa22b652f8afd9a2fc5ce8aa953470c7a0ffc138f93b96e60ff8adc5f661f02e7719d1a51e197fb1ead22238270cb9eb3ef5886055d" },
                { "zh-CN", "c57e1868dc0210f8b8a51719c96aa69970f9e96997f302085aef24100277e2842d17b4dfe6801324cf72d5e7d0239481f449d09cd1bc52af3ceb35273221fdb4" },
                { "zh-TW", "14e271cb149853833aa17c5086a0266b0f8f1b5f79271ba5d0047ad806ce0fde341e421b2c6eef711d51586f0267bf9e22aef12a73ebbcdd5fc17df5d6446506" }
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
            const string knownVersion = "124.0.2";
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
