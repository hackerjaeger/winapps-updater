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
            // https://ftp.mozilla.org/pub/firefox/releases/124.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "142b27dbbd04c91272f7a3d95f7c8cf8bf507e791dbb9b5daeb347c1399a1b50f7e7165f498b16c87b1cb5072b88e6f671863f77965ccf1db0b81691b162048d" },
                { "af", "cdf973e98c9114db8192f80e4d4f4eb848c58e39aa13f41dc42a6763e03117c818afa7ac466ec357d38bfe12fe0fdbca9feb33fc5a1f8f740995360090f097af" },
                { "an", "4d82da42af640e6cac0f9c8edf979bc6d09f74ae0a70aea656042d2165c934c8c43d975c3eac1a5756aaa7a59085cc94c3e3ebbe870bef265b8c7c5e05f667b8" },
                { "ar", "23bc22d807b8b4c3feab1f6470cd7109ba3ea1fde4d5b44df3a1aa9c79d4d555a6f0ba2ff5429693d764ff098a98a8769b3062611c7630d442fd36c3bd861007" },
                { "ast", "076ab23d50f588d6e96e39cb297d7f65e26b2e4e33bb9919d1f0033a249e2a7cba7913f81ff95384f01d753d9a63d0af8dfe47f06eac340794b6ed9e4e1a89d5" },
                { "az", "8a59ecd97b47a03b62b3d03175861dff26bc8d33476ef003e86b74f664849d592deda54e0e22e820b6dc662a3938c6227b38fa8fd1d89370c56eabe180268e03" },
                { "be", "b94926321906fc62913fe82b584ee56f97326eb916ea80b90dba35af677dc19e91ac5bfad951325dc53c9095f9ce0690ccd4bd5d2c4a737e6a902ba6e4d3878a" },
                { "bg", "908ba4aef96f8bcf71899e4960848accb78042550128c1a377586b51ec61c25fce83bf48a58d20a9c232df5557ea5713c0f92913a10f7981848a53f5c072dd4e" },
                { "bn", "2cbe02f9eb7568f314839b29e3d1a30d42891b91fe2c4c3c58a4b993ab42f272697748272325c8351d60e81198508cabeb31e33b5405ef474998468035183c0c" },
                { "br", "4ccf752e307496c2e1fc01a43ded8c1efd73518cec721e9b0f4e629d6d71f317d375281c880c1e0a884927c7c0646ec9c87bd35e4baefdb91bd6c37918f566cd" },
                { "bs", "707f1f391f79edd4cdc3f54af801223e7fbc7adc3c0fbd26d6644233b5c563fc2813b2ff4f5d95f54b155118e0862d9fb9821b8125ddeea4d395c72115faeac6" },
                { "ca", "8f6e5dced442cf66209cec86d85683ef503baa49d5242bf6870a6ddda0cfa9b6ae34cdafad0f75630685eb70ceac2ee02715484a018e53f20bd5c24cd65fe9d8" },
                { "cak", "2cfa5c448915796f005d61c2c66b7363097765ae5c3f360b61df1df41fef81b219128e800467d0a1ebba561bd38ad7dd3ed3e354c983459a928792fd5ca776e9" },
                { "cs", "24b36984fa0c3227ea66bba7c8d1525e3415e94acd254acc805224f62e571e6d67b36dfba8d5d1303564319b5ed99fcb5f8205de23c55db7b4aada1542ef5deb" },
                { "cy", "809b6766dacb03548379464182c564f208e4d3e5fe6e4babb1980c79bb529b90028bdb5a91bd35fa014211e4227dfe144d3e5a79ac44ac68456da051034672c3" },
                { "da", "326f996e2356c40384e319b4b013329f8968a60ea70e66c05a082de27c357428bf72f14208176aa9279029f6a9e7ce6675837ef676576fef1c534fffe50c4a43" },
                { "de", "b91d799571a2e87ea849d13b22a3701aef417ca1f608bf0711cf8f0689314f8c748a4596635d9c20d2fb679459f33a1d4bf3d556bce5cf595a219dabdcf398e9" },
                { "dsb", "bc232af1e1ba36a29c3c4bdf5b85d1e89eadd589c5364ce588400c4a661e03a5359540135ae85252a4ef2beb583623a4f990a8b80f9c11ffab044b86260a2932" },
                { "el", "21e8f4a91b4f8af10434e26d5ee76faa146eeee5298a957f9c732f718066dc50eb5a2c3543933bc33c9c6ce8c4be671d80718d28d07588e8e688438fc1b56c3a" },
                { "en-CA", "ae6d9f03dbcd59c2a002ce6c98055c1d7926da3acb71e212e00d0b3aaddeaf54bcedf204345bc4a02436eb17567684cac881bdb544ba209256b3f7ca764339f9" },
                { "en-GB", "fd9766dcb8e5550b5215abb175b75657136c40175d5a981697daaf5ff582a63f92f347637e14fec54ed68d177a7de1ba9b636476c04f5242092aedc2a044aaa6" },
                { "en-US", "37e250c3ad463f990e7d1b4210ebadda07b24769c5e05788d36e7da92003807c5c7a0fd77db2e7de092522787328c5ccb92e92b8b351ff7e90c4a31448619371" },
                { "eo", "b10a63f339731fb1c54b4802e256869978b435d3bd47f13739c815e464b709f5c6ddc27f5943f53def03c5e9c13e01d3b14a650a01380c900cb7ea1624abd7ed" },
                { "es-AR", "0aad2434e34718b16e112380de32ef35d443fd9ed4d42912b919688f853f073a571dffba2225de1b2364473cbc31f10b1fb7d0cf7efce9573c969de9e2f74c56" },
                { "es-CL", "e735a21a97e7a9f306336d457f6093799bc14319a21989c71ce51ca844ecdee25ccba29f81d7f41119b806b44246373b336a054e668519056d2bfcd8885983ed" },
                { "es-ES", "5f5f0f1587c68029a8e8f7b1e815a88af80dcaaae0e0c973351a8b722ae1d1f056b534cc5626a8aea4ad8a4c0089c552d5821cd9440d96222d749624445983d9" },
                { "es-MX", "03adae4a627bc3caa17f3e26833013950f6e770eb9d9959dae1a86d97e49aea8274bdd2c2199d7ead178085a9362b57a316ccf780dcc76b9c895e3f7294e9b9d" },
                { "et", "a22f7dae3cc15fa785d61e13ca5f6c21173782ac368f768f86de3e142f705a2c9eef3c5ad026447c1e287652c7891787ce343a731e31ca4af6afd90a0da0f916" },
                { "eu", "859acf48fe4db9042bd16f8d7025eed4e8eff445f53609bc511ac92af445690ad90b7aa9d7381e5ff8f8a72a1a87247063ed19f4f3e74c7d88e5879bcfb63bf1" },
                { "fa", "c06fd0e8c7cdbfcde3de9245fb521776bc2ffab925b7afc5fc7d05236fc7f0c20b6afa51398ede2de7e9c6f2842978cd1785a52c77dcb505466b7f7191f12f27" },
                { "ff", "dd1ea73e8a6905492c59cf301174149d562306c23c8a8e3037790902426bc66157963012325a742d6b2a04336d5aeb188ccac614fd423f32dd5a5be1caf2db93" },
                { "fi", "c28ebc84d1c3218d5bf084fa1ebc3e13267272d47dc94d892e15bd9a834db0cc00385229038ce71f905655755ee694665eea6776001baef350d3dd34be423a7e" },
                { "fr", "b0b03780626453574cf081b383bc7bc566316de026b0806244f49a523ba0cd034039cd92831f551031a16b46b62f7ab52187d39ba6102d6930b307615c9cef55" },
                { "fur", "5259292e8d6c5954005bd674d50de2d0fc5b10a2a256c5830a8bb947666c67e3e7f836a86e0ff6f714e2122e8238d9254ba90ec8c595786d6b49d5905c4dbcda" },
                { "fy-NL", "8fd80704c050c695c16dc0d8ebdb8e15e987e80787458f01fe7b3f8c0a4285b1343acd51a170726f74eee16dafdb09ae5572bf973d2422f854f395e27ac70b9a" },
                { "ga-IE", "660e2b2942f8734b2541a07dff6fdba9b67da02ee34100255805ca9366b50c7f807e24b01d699c10471c96666cc805779ab5e7d6872eef1bf350be9c54174958" },
                { "gd", "570a1a0e4e9fc9a33b89d01569579709fe8167326cd7b97ae098afea45aa145af7d5f8ee53bd6afd6a107c3fd04ce145fb6d21f71cc8d7cc9ede087b303b85ee" },
                { "gl", "225ff591a990be471e88963d3900f0cf182b952d789eac6337c026c5b7d85ba4f25e80eeb5a7d70fbbc4b6134fff01cf9b6abd8f7c9e532d35997a8cd29335b2" },
                { "gn", "d2b49cb2eef8c51116a532f622a54d4a92eb041b266fbcd382c6545fc6448478e08bb09f1c44fa18e811a9e3ccbc40cba801aae4ed5b3cda75ce7961ab322e40" },
                { "gu-IN", "b2f37d61a47c876eda64b9930ebe6e828610ef06f36754ce29c28e7783c4a8a6675ff2bab6057832be1d8b0bf50dacfa0bdf4faa46e94a20683bf523dc106507" },
                { "he", "8fbb6e1b0b951501ce4e51ad4521c9e60fa39e8eccaede8c89d7427c25d8a52eec2a35cdfc19aed3d696897740175f4c65a9462f946d6cd5bb9221f55f7b7f29" },
                { "hi-IN", "2f47245d378b0a27ba519053d6d148af4f4a758e6472152d092cc9e5fd08435a4f530cb70b34d6c339a414b606d181da0a0e1d04a3c70db007f327e4472aba0b" },
                { "hr", "289ee98a36bda9b5c161d898ceec5a3841a222c1378fe4af021624b9eea30468948d923c57185aff1cf2537f3001a2358d6944c9ad47d6cc1acd44f2ed3b52f8" },
                { "hsb", "1f281e0c5576181384d98f2245b6b397250ba10b405f336750cd67fac552f6f1d807caf4d256179acda880fec9e6db4802a25f706f5b754d90a76b844d0dc76a" },
                { "hu", "ce6549115e0651bbcb895e9f2762924082d8fef097cc151aa2f01cbc051df44926b0c089867021bdf85394aa6f588af88c853606ca3d9af212c3382248264433" },
                { "hy-AM", "1b5634f587d6eb387e1012d694a7bd2a86aaf026dd160e11604679e3ac0eee6c765e2e7280b60fbfbd897fb48d716fedb5656daf6dc424295a762673d2725578" },
                { "ia", "282c175c1e8d31bc5d9920869d5e18cd0fa62337514af01247d51e6f6d348f9c75b07d7954fbc01300056ba0dce465cee773ca41c9a171af369b495bdd97cdc8" },
                { "id", "d42d5fd00792a2cd9379b6769a5223c40a681000de23b9f8cdb6d6548e9ba5aeba4d6673738c97c154f4a7eb8e28dfe86ca442f3f70e53c31bac9d5c941b6ade" },
                { "is", "1e85b1698c6359cce74d470ae180d303f60a172e78b0dd0f7371025c2fe8add2e5754732e91472c26be31db478ed6e0487351ce6621b14878f53784a053f2ab5" },
                { "it", "a078cbdb4351359896a5f15d4d6a024402e9e5190a0d2a38c5a0d85e3ca22ea7c1ae88edb84a41677c04c9faad7161a7e4e3067557068a6cd6ca98daf296b900" },
                { "ja", "3140fb8fa7db9e6105f1d42b34eaa36018d09b0693387ef7b0fc46105228584e9077661a3eaacaebee81848af02cd8d13ed2008406d252b10a414814ead7f154" },
                { "ka", "27e7a9e433bdbecbf581e4c19d2317e2368b51d6d98e8d04808ec53bb4b98b6db1ac6f58c253e0a7a0e83f071f60249822aef2cfbd470305cb539cc511530495" },
                { "kab", "6df480fdb7194db408151927544fbcdf9922a761990439036d2508037a097c051a52b068c8ad9f196b9b403c19b3cc2ddbbdf9b13fe703da6ec2c640c1994059" },
                { "kk", "12106cc0b4e84158711f9e1afb97b4823f4d13fb6c389bdfe9254c8c0aee4d83212d57b63bfe808d9951d4aa92135c257e355bbbb11c672d3bfb0f16a0779b37" },
                { "km", "ce371d77f830af1740ffa0fbc9e18d995e693a909475dfea69be9d56c29b8d44ab590ec77eaee1e1a7d7d87ed6a43ea653f4ed53a501fa56018bf33eabae6f2b" },
                { "kn", "3185e6cb7b61624535d70e6befc6d121205102dd922ec380bf044da0ee676392beed675e9846d3a2c3fa8d2e1693a4c261d50104fa957f6e7c1898a8e2a8a5e5" },
                { "ko", "4e40a74c4c0a657aadce1f7d5581bdeaebfd423fda6ea83f2f9e57f6e26f3bbebe16e56c03d2348626af27de8b72a9c5bdee71bc95669a237ab254b7c2a55925" },
                { "lij", "d632b174e0c9c6b8c1910329adc125c145fcef92a02aeaf707e09655478be48c12aa5832dddfb9d843a10a76ae775732c6ed8126f139c918298fddb8bb18e7c7" },
                { "lt", "3f416c6170bfc4b584ab0bd4d0ad6ec6367a1b8789bafabd55590902846fefe8d7b6cab3c642609f81f2db6d6ebf6527b02b1f6b144a7671af4fa2980a43c09d" },
                { "lv", "66e2318f51a2b77bc76c0709ed19db2fa096c15e7c901f7ea0eccaefc28e8e07c29052371ccfa74a483a489f7489f9ef20e7b81c5f761b7e944c106440487e72" },
                { "mk", "4cdc6cceb963def3887c697b9a70697e4f7e78be0d6155a85b8fd5079b5f114ef5e15f400a782034a6813f9dafc10a8d8f811865ffc4c2f31a81e8bb279a4d1f" },
                { "mr", "32e6c3c07beaafbbe52fdcdd252a05a421a3f89fd4ee406ed897211c1b1272bbb526692a96d65c6881b8ba6b8848ff32652ac57209db5a0537e07d1d546ad713" },
                { "ms", "63e98dd99a751b55f84de727e1de3eb1e5c56c0e91c557ef490d553205195853049eb38f8bb2390b5b16327b3297601ab61af04962b245966028577772586ad5" },
                { "my", "eca999c0204a41bb39d1128cbdb81626a1f8b910d7124fddf40c896fc1f2802e1e913f92bb91398619569a3471f9f6d69fbad2beb0bebb9306806154b59d2968" },
                { "nb-NO", "2d1d04e4c88453d16c8651ad2a1d68d5d300400ecdcfc891363f88dcb3250e1fd28a42e69c15fa02521afd81893d9c49af3dd36edd8d2b52e11ba9f6176eda29" },
                { "ne-NP", "be7ca1a5e50fe39f8fb7e9074d937c231632f2e56c6067781afe2fc515cac7f22a4a809ae0ad71562ed98633b035e301ccbbeda9817db07cae8e28741c483bbf" },
                { "nl", "eab036d018320e6cfb497451ed2c0619dabbc723d47c027fe24d34f2254baa0edddab065fd3fd71fdaf6e8e40a0cf8e5fa81839b114fa15757321ed2112504b8" },
                { "nn-NO", "5d5530aaa093af331d9a11b596319eb3711ec92ea696f5025d7c641a8aeaf64027654d5468c433ea6c49244fbcef2ab7f0a64cb44d8a03a3678588be365e1c01" },
                { "oc", "96b5aac69cd6725411e2800cf1dda786a972a877e9c94b719e6ce8f87d47ba06094417e0dd0c4109810f7708a789390af1edb7fea2f1d6aa750b8de7dd72f626" },
                { "pa-IN", "84da6f08f46293d569919ad06a6b44fb39ab87e8ff36da9fa3a3ab804353eb7fc99cfd98a5959049f2ad60daadf5b2571ab37d9c0ee65abe6be5de132fd2e2a0" },
                { "pl", "6d4a60dc7fb48784ce2465eb3ef4ebdb8ec4e33e845ba3d068d3848b47a8eb1aae0c7a5bb903495163686a110b950c309d9220a178be68f9b3bbda45745de5f0" },
                { "pt-BR", "8ac9dfe74197828909e135fbb87750c44f78f16bc94e102d14b84b019754fe7db33897933d4529ace26ca8c049e6c0d6cd32f8c21c8fb24942c9c8e7fe5bc352" },
                { "pt-PT", "f343b318dd601a6a80e5a0ac6961a4bdca34ae38003a40061f75233e5faa64ef013fe551f49da31fcbbde3244fb438a189616c118be1c1da6ecba5d0d3209c0c" },
                { "rm", "8b2fbe016722f1f81bc58717ebb7ad4e9de68abe58c325c6445730c4a91b1d9fd7f59e939774b8cdea36653eecdd746a197d8431c21cf255b794cf1afed0ee3c" },
                { "ro", "08769fa49d23945f70f070736aee20b6d60e741728fe2507c015bd944a507f71fbf71781190f1b9b46365c45205fe97297bc69704d8fad576d9b4435f26f49fa" },
                { "ru", "46804542fcd9f56a26aec91678daeacee921406564ebd8e5f35b32c987d37da88d09239d74335231e2dd6f870d6ef353658240d2c8df3ca4c4410f0f5f053232" },
                { "sat", "07e724c201adc3fbad7d3ee3b5e3755676a09c62399b1a78e7bbfb38dc8ee5751f21f14b96e7eab14119aea14a8aa4a2e9a1ae840c9dd164bcde7116688ecc44" },
                { "sc", "50bd966586dd10351226d9b7dba4b7fae8585fdb341bb1e4881c60f8f4d654bafd693eef948e70fcd3a6dea4f7ef608fcf92e4f54cb51954a4670548bec45d1a" },
                { "sco", "84f347a3b785a294a6132fd8ada2332992bbc976bb7ed4afca0e4676ebbfe30187c2adf2388fe62deda579aada92d33c1a4dc34f74351cbc87138f9c67d455aa" },
                { "si", "1071f24a41463781f376d113e6987122a104f97df955ded4917bd5b98bee65c14a0090c43144f85504407bcc6c9a4c77002767dc2f50245ad8cf228d1def836b" },
                { "sk", "57e231f14df97be9f43c0781cdd756b497292d44853874926ee7338a1bd23e6fb912fd536d819b832518e27272c2ef5f506df2ca4a1d439c5e3c90691802ff7e" },
                { "sl", "22ef35e854bd7dd4377bf124173f3518dd9ae83f040e62d19b12eec1ed3de5ec9395693d76064e6c8a2fe43fe820d5c23b824db08de4cb4b1012bdbf071e519e" },
                { "son", "531f132af60069cbc901bf5e7f24302ec4e4f55b950a53ccc9c5f6ce7d13a0f786333c3589560bcf306f2b84488b43975bdd31000c988a0687b5ea021451cb6c" },
                { "sq", "2fe971390ca80de2ee1ac2327c0e8885e8044e7f30f81ace727156bad595fcb319f8247e355bdc62a65e38525162450969d00a48d6ad99803b890abd1e025182" },
                { "sr", "54433c090ea81d2e3c2a02b175e1df4c6311604793050e3f2eb6f5df2fc925232eacbcc7035518661705fd8990dae67c93e700d8cbc4b0d451e09d0d2661af12" },
                { "sv-SE", "c9868e1951df9ccfdb36ed1cbf1c7a56301e6b8f93b39afd2f4d326b1d95db7d01f13a97faec423da6b1cc835494f83b1e0cf817981e91b66a5eb9ba034eabe3" },
                { "szl", "c6ecec18698d9a9c99dd004cf8d280610628ef5ffed57feddc9e0c6c1a8660d39d60b162ea5a2e1479032455e4995b57b2ba99fc5659b2111e4fedb447246a70" },
                { "ta", "e362b8a964ad5f89e68c4329b6c532e56525c6e4e3b54bd22e8e107f8035ce987464f10a687ec00301939518b8c2673eb17c1df5d0e707e39980b45338e1800c" },
                { "te", "4bb2fd37503a04f74ee72ac4a3c34ad4abcb11a1eb0be650e0d243769ae01de2ea1099eeb9afe8b7930467a529de1cdf2b2205cc83f99ce5a3f428928ef25bb7" },
                { "tg", "2b800b4f5c2c706772de74fafe729b8b612c05a2a6ab286c3f5dd43c85410d81db1eadd66a2a10cf427d96778fa7a2e076daa5d9e955d1aa03b68d44a7f930f5" },
                { "th", "6f47ac0a3b364292a30c95805f275e3f820f2c81e624d0138d667542d4b6fabe0cb24f6ad05035b2618d70fc71a2d5843623fede0938723a65462b2090135c9c" },
                { "tl", "7f88a3ee7619d41117cca01e94516756243f91a84f6dd273df5d056a857b2e9a702c534a379576ee8f5cd155d7463decedafa727139a7dd12bbbef23116be8d4" },
                { "tr", "90f7f548a70e608f1151a5ebbb99715724e996f0359b6de95deffcbb9ca5119b7c23227fc5b883a5224f15a7d620ee84b600ff77a99cc4d9a9c3e7415fa7e32f" },
                { "trs", "1bb508c36d33f44acb9bb7842df79de07f696425c9f88c8888f2467658c5791f686d433fa146e21de23ce6edef0be3c83c64e6577ae19c3cdbaff3194667c6eb" },
                { "uk", "a5346f92001cbcfe0ebd3d5d2fc9d79dbfc7e508d12987fc28f688e129bc30e4b23587851a70745acd72227f95b462f661754584ffc570a34bd93b042b22f6a8" },
                { "ur", "1a8e8e4b506cd79e6dd2981959531a1fd0a8c88bb93d4fcde25d22e79664a3943f7da193956ce6a85b7eae248992f91aad4858012c334cf6a7615248db675d14" },
                { "uz", "578c67cda65163a82e7a3668def2b421f7e24fd82a593acac7a9a84af5a3f86896684f4885f88ac882236a685ffeb190861dea037d03f56e4d218907b201eb46" },
                { "vi", "2f6e206a9488c3ef79140b096fdef61918a8bde45449826f65524fe8ea2a98e3ba74373caf704071072f2edff603b3e17c632420d7af084b5c003839773fa022" },
                { "xh", "81327eed068ae93f04f2ae859149b65b9e55596cca4d8e980d660291021efe7f06fec8c5c155aafef0c08656e1d62b72baf7fc872000899537e5825288ef560c" },
                { "zh-CN", "e931ed3019e1981d9412d01c2130ce1f7e930767cbc3da92d2218881ae538b83db373fbce839dbd3e4d2606c96d7198987985115fd3e49826ca53b3bd8f8e576" },
                { "zh-TW", "fb70463681fad75d54caff89179246d3dd5ffaaa798beda7f333d31c6f7720d34f6e3c867aeb6616fe31fe46194edd68203c673c8a4295a62381eee4a9879209" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/124.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "47e956453ed140de304ebbc9987a26b1c2df205f0145c47727e1e62a867e3cde3424c6e2b9537aac22b40a6312b85e92da0209583d562c84373c3c63aca5c235" },
                { "af", "9b46905f0abe9b8a3b8cb6989b27490331acbd9ce38b5e939eff1f136131b8dbd1ed1168ab408e3187c7e68b152a5680df73370f5736ddb1551317a574027308" },
                { "an", "dc5667f4e0d1f46b2e9c2624f34b7f2ce8a07fe74b01911ff4a9e3a88f09f9fdf0d5cd3f0e25996b25fb06ec84c1c6318c577b4cda9e5c83d64ebb01c8814c60" },
                { "ar", "f0ca5f90f44aaa4ca953bafa9720c8734b0aa7669e59a2bfa944490f3dd188b7371e96888a1ed5b003192eba477beec3e21381bde5b411791aae8936b51e90b0" },
                { "ast", "7202ddf52bbec73e0f3741e5fd889a3cae917e09e1e4f42388c151daed4689f4fc6ba775effe84fef9b3afbeda5852642b44b18833a1d4af8fe051eda9046af5" },
                { "az", "818c5e38ea4083c6279ec2935a484f8e6a961f1a58b997a70dc2a0e59cef23568c58fcca7d2689000db09055b3894a8763ed37679822242f1082386d2e95f78b" },
                { "be", "85174254d32ae386f2effe61e7d5aa1bfd5255071ff608ea26eb9fcdef5393e8e2396f447b205060ebcc2250cd44d290477141c8a518c2ed14d7a9c0db30fc1a" },
                { "bg", "16fda723915c880268d3edc5f17e83e12a12d4b131de17a7184e99f31a32a7047a55e0c9298dff081864e177bb0f32a87ca7bc7bb288dd095c7cf3c3e1a4975c" },
                { "bn", "aa578124c83d420868c60fd706160c3c8bf2ceb46339e7dea49e88f04fb939cc7a3e05047b829c3801738cf2aa098e90c126670b63b0e04dec514fe7f067b3af" },
                { "br", "4a58cb2e754474969f337a78e3bfb58112ab104a2369b1d066b2d75e53be98548cf446e62e477097cce01ef1d9ffc46c8ba7b8d91ffd1fd7c4caa513b20f7219" },
                { "bs", "3da8922949f92a14b7fb272d953167f2de2f17d31ea8c83ec40d1bd1b96ed57ec78a1de2e8c62d13baeca27b29de2c162d0d636069c7626d3fdb23b09fefcc66" },
                { "ca", "16b4027f311a871c6b1b6e6985e815cecf5eb6ccf09ffe8e30ec440b5f27f593ed91d31da9165a51795a23367edda04a8c7267e1e4ee705cf1f6ef6dfeb94edb" },
                { "cak", "e9713f2bb7a90241782743445d3b8afd3906899d3560fe9f292d69868b19bc59e8c1e042153bc911169158a2b3bf5d4e069b0925ccc9f45c264e67da971dfd0c" },
                { "cs", "e0274cb4180ddab5ccdd03a67959c163014f7765d497b22bb7258208e59cee7e77b62d339a9120531b200534dfd8f60d8bd6b2d17680cb32d9b1ee4cda4aadb4" },
                { "cy", "521848b9f71b905a666eaadc6136f8260d5105fecbb15d4ee52b842a586c61e9608696375e2667dd72e412fc7dc82c893c3fc539918c620e17bc43f692f60946" },
                { "da", "ee58a3e5a9c41d4bdba7b35ab323006fb083bcd78f4d37d24d4622f04c74b50629fab5cef591ad15d1d901b57a13def3fa8a4643790b26a907da09ae713970aa" },
                { "de", "f5b1294098a40c636d6621c3cb304c9d0c22f07212d276cdf900e00aa4fd3f3bfc71f11e0a5ea07e24b7443b8e4183abdb24665cbcfe508e059caf5b13e66d46" },
                { "dsb", "38037646176bf6ce97803cf829b33298d2dd1b66894647e0a1bd7ff7f9241bbc5e5e913c60e09cac2434fe63e83ebd4f1aee36f8de191074b8f2fe2e5aadcb89" },
                { "el", "bb0a90ce2e8d9fd55f4169821923b173875ed78f53336b07c4e99e604b430525739ce4d4dd16967abfdb82f73593bb2f264c0065092b860aab2e778aa522ec89" },
                { "en-CA", "50e0e4b5f42514644c945f9b6dd74787d0c56dc5b564dba27e63d61c3b8e03e10bc70ec54bc5e4e7662773c4674f2af5624eb3c6320a1ddef3e654b90313017d" },
                { "en-GB", "36198fc9492f0b4a1a6a787a8e507a0099d0850c731c93d93a2f4507b727b6f534b33b4197aeb56f532404ad677b93247c84d27dc9e8369caa8dac0150e2c1d5" },
                { "en-US", "c3b31d4559988e743b74505dbcdf7187c9661c57328f24b6e47d966cbd5e2b135c242c3cc2e27e8b5e377248eb6a6f2ccccad4758baa965b6950db7f9d0e6745" },
                { "eo", "a854a6ba3771b2088aaf6e23cdda4a47ac9292b67f106992137d9a2614ce6c5962eb3b3d51aa596f50e39b54e947d3b0f86c7315f9d544ba6d7657df6d19af67" },
                { "es-AR", "a52d588fa97479b489cb417b1734c73220fc51bfea0950d8d7dc40a21b9f01c2de8cf5bf4885f3469a5622f59760d35ccc2492f80997dd0f8d9b9d1f9c483581" },
                { "es-CL", "7b89118418ef0799853ea01b438304792003ee05a89ff6ef69a093c627081e7735ecbcfce20348a1312ab0159d1759b956f54798833e6e915ca9671d799b8b6a" },
                { "es-ES", "aca545ad0df864a49be3fc90f731a5c82014dfc629c8126aab280f7880390566719affd01acf33a36ccbdd48834c9a23f20c4b56221d3e59841eb273ed12d33f" },
                { "es-MX", "250b55b64a71df59dd02f723214af59fe95999920d18f71e98167b78c4adf725be0add78743a974d1cb3aa2f92ebd8c2414df6db58fd7cd6d5b5579576b41e92" },
                { "et", "4ca497b7db4331d163e00d893b290f30cfe39a24bedc6941f0e65f7c20ac573facd8b9c370cb33b4db250ab567949ebb107ad8bba7284164369f195ce4c42372" },
                { "eu", "fe4034e8db444e0db3621242ebba014ddffab88e73211f5409831992a9a933bbfeef0db5af0ab333b94364167bc2a99878328ca518472d0b7c922de52e68c67a" },
                { "fa", "3f547b2fe74a1248a7819ea1e78a4486cba6518dd543b280547fa612794367bbe19e43b95ded3e1d1fac4a25ee6346493c6377e4f3b45c38e85f8b8c5af65f41" },
                { "ff", "55ec53778e594ace04e609590dd9162b5bda3f763221eddde2535b07b85b9aa8e8f2775b84c02bcb7c8419db3ab1bbf975bb27f396e7b36bd7a8e3da93237b67" },
                { "fi", "5ec4b7ca0f9e601d37b0b36b608f8abe00ef8419018a27de095bc09f7f8de142a9572d79282000feb99b4887534ae2ffcd84ea6622a8904203dc056963b094d5" },
                { "fr", "ffd0eaae269fdef207a6caa5f09a1cfc83dc242f092b0cb6c55721cf3b80047080f29642e1cddc5f4e9350140818b99f776c738e6c3a2dc59216851a51757cc0" },
                { "fur", "019e6d2878df25c983ba5f8c6918dc3ecadf040e26240bab189c0572eaed29bf228414c6d822be0136c1de87189726a14cf4b35fde5e1aedfb4fd1f865d2bf30" },
                { "fy-NL", "f1507832d237ad2cf47b18a222be211ca175303a4e1eab7198b7dc76066ad80de1d3d5d05e9fc6e8efd25bbcd44912b1bb84be3968b6b06ad322d29efce037b6" },
                { "ga-IE", "624112aebe6c3be08c775dd09f2b573edb08a4f98351ac74ef0748d6aef9aae8d908f635e6b8fdcdfb42a4bb79822c57fcf35ca943c81376e2b015048f9f5585" },
                { "gd", "006959e59a3b72034ba1538ed39014dde8f6ec464e9e33d794e9b6cbdf702d6ea3a9cb16c7e1f4489111632dcf2958b1a4d346bd82c2d7fa2c1e61befa7d9f7e" },
                { "gl", "9a77146fe3e81735aad24eefc951e547a5c458b45df65cb0096d76928683d8eb429cfe7f8bc888889b5d7e93b8dc4b58237233b7f8aac9f91b7e185954245e8c" },
                { "gn", "68c98e804d2156b775ff0aedc1ad24a351f828c16bc8bb23ddcfda2d3a87943a919cd89ee0ad4c71de7b0c32a8a8c95e67c21eb24460d456a8e24eb23944c8c0" },
                { "gu-IN", "f5020688fbe59821e41d9ddba85f95e81ef07964a8c9ed4b45eec98d2913dc8296173fe5c9d145bbad9c60c7378356485690f03578b2923c0efa0c70ef34b9df" },
                { "he", "cb7be90575bf304c468bf5e65f79c045892002a7bee984e79be63ce539b7dcbaad2c542e745858df3c6df1b78eac4262740138ceacf89094c51de5dcbdedc176" },
                { "hi-IN", "0e303ff443b47c967929abc2a3ddfb24aeda213ed69b31368a7a564dda6a284b5de4221ebf219a2757d5d9d212ad8f6090a7e24096753572b28c16140feeaa16" },
                { "hr", "366128c2a580fbbac51d1a15bc4e9322114091029054f79248a49e7d0ede1ab1c3d24f1d3f680571ccc0bc634524bf58bb25069e22f306a3a2dc447d6936e828" },
                { "hsb", "f3809e735d2b0a4e319b43387a450c9fa8286fe491f49859d9d4f22a9584e24fc2effef53c73ce2119eaed4fdb6653b31702edfd0a7f963e35e2f1d3fa2f83fa" },
                { "hu", "e11cd0033529137bcf2c6cefe4830aca2b4d545f6933d261044327d43af3095982874c821c011515d1f49fbd343bbd701eb2d3ad9eb7a404ace8aeac0ebc22f4" },
                { "hy-AM", "16321a006174592d2d7531baff37e5a334cbdfc3da1c50cd254e6ec5cd12b72e34e6e27c4c7e6b07e1aeb00431290d5e5c0a1281d74d33e4f6048e663cc09682" },
                { "ia", "1b273712a0df60087d5bdb080d318158831cd3f49a4769fd83b3e106b6db5b68ffb117dbd4033edcaf9124de9773c01642fc33231a3cf2466c78082552356901" },
                { "id", "d608809d87e8caec9ffc2b9a559b9c23d42502da0be2b51761c11d2303f49c896bdd1d8a859f0e2dd3672066fcce6d3d3873362855a8566bc70812220330e817" },
                { "is", "1b7760b00820a00a9834854a1c071f3332827ea56a8ee974f588bf0ad8a8797b230987edf0390802552d55f7a6ef9e28c3b621bcc53481dbf165d2a7faadc169" },
                { "it", "e508b1bd785df90f0c4a3a779d53aa004cd39a46de8508b6a382e68e90afadc037d01bb7b8ccc2696b5d2d98b3b198bf279752cfed62ec514923e64127ba1801" },
                { "ja", "6f5e135271b4b327a8468a3be650a13b3f88773b12932056b7dab28870ce336b06eb4a7358b79495dc3e5a4c2da79b7f31be1f5cebeaa3b96b9f425dc479335d" },
                { "ka", "2302600a35455bbde2f2623b889f537c48d130d6f4fd410421240acc7890981a0cde769f3f87a2d3adc1d85fdb01eaabb3b7e0a89aac99de80d567c08c866108" },
                { "kab", "6585583f071a490b536024de0aee19878eab216a7f1ebfc46b75db0018722479ec0611d32ecc2412b8ff0c622bde4908484d800854432108141e878438aad9ea" },
                { "kk", "184fe6c945075282f9976e8b3d49597d29480ffaab13078b28d2e76b0a34ba8f73176fb3b916e1734dd8260aa6af1134b145ec1a46df3c7ee843ced428a67e28" },
                { "km", "e9a9c291c0707cd85c237430d504b67230d2853ac23b1531ecee06cbe4ca67d8748d19ca3cccb70bf5a37a8e9568f8b728115f13f76cd094ab96c6936a23b462" },
                { "kn", "f945f0cb4ca0651e423844fb10e74bb56a44f6f55878e3158b57b1285a3b36e312c05e1783eadb6377b5e619f3afef9c42ac4361749680410d3e5c288950a8a5" },
                { "ko", "f392165fe950c0f27430bf625f39074031d7391023e662a0f76e9e040762d55fd0a755cdffac147d35c7df69ca100bd00cb0877a34563df0dd68ad00b3b05b8c" },
                { "lij", "145706a1de06c18246200e07af7c8bfe4085d9c48e04dbc1491140b6bb20a496ed1734a55557acf4eb3e7ec534f68987c9f1784af53e502b7def24408380c330" },
                { "lt", "185076721d52374be121d76251cb943857df72dd7a9221964d730343dae494e4c8811fad3aa1df396610bf729915bbeedc1451f758ebf6e6f931930ce2238efd" },
                { "lv", "96e51ab005781a82baf0aec53c87476e0a81ce22af774e6bfc56fe7848bc74afba835aa92f6c571aed908cd4441a3af17bd57894ead87e797a58467f42adae8a" },
                { "mk", "83cd5e0a5500e27e5aa6c32c9d1df483e3c1d2601272cf7210b267627577f5fa08caaabfbdeef47d01ec6655eb6c3ff31cdd46b1e1715bf33b54c4fcf123e62b" },
                { "mr", "44464d826e65e294ca3497539bc60a3035551162288d7651eeb573f332d07396e12bff3af9cb1ace9f4af94daf527f5cd703caf8d931e5049eca2c74644ce29a" },
                { "ms", "bff73fe29896ec092e942c383e2957895bee1210d91dc32ed4e043ffe356eedaefe8a7c9fc9ae986851630c90925d06dc63f555f600be2c9b3e8c8e72f175de1" },
                { "my", "31137132bb7a575fc8d3dde5d5bac31570730fc72d0bdf23e71c40ef3ff1049bf1028e33a38eb341acac10d9be060764dda839327f5e38122a93698358b38f55" },
                { "nb-NO", "39d78ad3d81109e2aefb55aaf59fd29cd1e7fc75d3bf955e41ce3a9432640efb15d25bf02a0cbe84caa1d8254ebee240e5f57d2323c8da7d71612cbec118903c" },
                { "ne-NP", "87655cef9691c2a84f7bd8452deb506c3ca4328fc6b3418f14bdfb4dd1cf6907a907719d390b0f0877cc3bdaf004d533311f65cc0b6b09c94bd368908ac98f8a" },
                { "nl", "671adee3829fae7d014a50ffd1e6c7d85174af5a842a8763e0184cabd65321d128ad4b1a8225f03fc81d119444452ea7cc6cbe246e5178a9b8fcfb497770ae3b" },
                { "nn-NO", "597d13508cdb6a74b715036d67da397a01a9c92b9530cb9138c5913677b3418e09391a584f3be911a94037715230129950ac2526f59ce1844ea4e0ac5b224b81" },
                { "oc", "ad9c053dcf4dd0e31997ccc61c25ca210bbcd166adecadbe4ec35429bb2984bad07bd203475b4c07ca5c146a7caff2c0789d6b9afddd8da8374cc49eea379091" },
                { "pa-IN", "9ff7065a056cb307a0392179c10bd670b10c627c1449626a3e2ef474d166d3379cacb8f0318d22270dc8193f9a4606364ee95dcbfc26f688169e473839cda762" },
                { "pl", "53b1626324a35a9ff97f12f5e908cc209f2d6da440ceb12c83342bcdb29d4adfc0f836e999fa580b496d1306ea68da1530726df76bb5c337140744a72a859816" },
                { "pt-BR", "a9640f75e5979ab6281024bc3208f48b8547c9593b009fd20bb409055bd5e36a9a3cf8acfb56a1b8f72e77b32aa6d7e381881d76d79c4c818d4ee53d93c4948c" },
                { "pt-PT", "8744b73327a8e84cee184470aa7da52527123e367e063ebd9a756b99ab2b2c49f7f05cd497274af8f5be6de48f57dbd9445a226466ee7dc61453b7e102ca5b8b" },
                { "rm", "fa8fdb476a3278da881f9617df91925c60192a408f23a0a3685f9caa2740b83f8812c8acf8af3e49c1cb42cd79b74b683f2c7b731e2ce8a18a818523ef6c27a1" },
                { "ro", "3d25ee4f20c7cfcf6e5ada8032d63e08e4893a3696cc10dd0ca7d4b7d618f009b759fe0ac37d9673a81a60c8327e7fd6edb8ca99ecb27f8aee2e5bc8cc758feb" },
                { "ru", "8db35a7339ee004fd383de23125c9699ed9d54fd1f918ec2a523df882b4dafb1af394eee47a7ecc2399f22e85cc2b160531cb9a9bce7c0b512a6dfda81672b01" },
                { "sat", "904edbbdf2c1243b0b844750441e33e6785dbacc164b056b85532ab9380d3175b95f1a3b3c4cfdf38455a0731b743a6f7f4f3b216388af58c22be8f38c9d3c8a" },
                { "sc", "6f93a5b818b8170012101e7751be1b0c8ffc740cfe99b06cfbef09b386f61c87908ae78843d185f733d1947612c4c41b136f0f6711d2bce4c6f78ec3557ea28c" },
                { "sco", "61c4629caa75107ac86e63f686176c4a080aeb44b228ed5204010fdb6576cdb1fe471a19895b73668761a53b0dac592373210b6fc6b7794a96272e52438eaa98" },
                { "si", "506eb2b11aad767af49a2501c20d5f107e0c30a3e82847dd752f55bbb055eb809ccb24b4311ef61cde5dfdb65dd2596d0c63eb4f371b996aaee30ab3bccae628" },
                { "sk", "6e4a5e59c2bd9032985dd9e504f70e30f02975b74d52f5d2687ef4dcb9ca8916563a04421e78d6bb562675a391127e2660f99c961dcf431bb3816069e2c63dc8" },
                { "sl", "917b3e0bfab027488a9160538cd9b15c21e16b73f41254bf9a993281745f98788c373cf25f517a1c59d59984d8da00c59c6d147708ef55b624f31d12e9934fcb" },
                { "son", "9ed99170a55d423d8c973945108d73ce736658d866a349c3f327bc04a114366b977335f7d53907f68868491f9cfe11a7330a28c9d0975959039df7b1aa6a686f" },
                { "sq", "f5342ba0808e707abb1c85a49e1f176b5a192667c2ce23e9878a68a66da2602e162c0b546ebf8e5a9ceb2bb8007addd32dac9bdd05fa4cf7c5e66dbe87d7c63e" },
                { "sr", "de026f449c77eaa17e4e0b9fd6c6aabc39d175fc380ffab2008f28b7e7cd11e2d7929eca7b65dbfd727e68f96a1d4ab9f691468e806d201053e6a5eb37bf4b3f" },
                { "sv-SE", "5065aaea1e60c6ce47362fe6e7d4f207c90356af77cd774907010b2f1582d2c956c557127446c92c6005694efa849d42dc7f6afda91e5ffa01f0b0b56d5fa736" },
                { "szl", "bb89a923edb5ea857d22f0f4b276f3e5a26228c48b9f7c6e4c62b71b92664e5a19aef75166248280f59803f25da71ce93f7c4446ad69df2405b01096c2f0f47c" },
                { "ta", "0bc9191d22ce39e5a3f1540fefdc64229a0a87120dc4ec358c675c4fc950ac31479f92b540799e0ee546202c9da4359d4302fcf0c9c6f705645a61f974d16dee" },
                { "te", "fb5e2abedc252aab3a2d3dccc4d7905c1cd5c29a6e73350250b8c01c8717834acd237de740d4cd8dab5cf071f268c90ef1a8db83a51eefda1c8b379f72514119" },
                { "tg", "af4d0afc1188ac09cc968ceb55cd4499cec91fb6c8ff25bef9e820ad7828173974cd2f13476281d971ee2c8cf4318730193f9451a53e9b7d1646f0734de2d9ff" },
                { "th", "3b9d2b832f896be77d27863022104ae89569b153b57c5277f81a1f475b0061c39ea2976c92b1703e5e43a5a233d88f92a46ccca8433b4c7313483c3a6f47fde5" },
                { "tl", "1d5754d1da32d6b7fe4b21c5ba820037182840ca46ee7104ca5dc6006234317a73cf88e799b57222aed4cfae7c6ca581133948ae5d4a35ce9efa70c24512ae15" },
                { "tr", "dd1acd2f318309bd1d34d02db154849503645a52199a7139bc6bb5795fec8cd4cfedfdd0244ad653fd17045871b1fd7d4e769771dcb748435f9f1b559e530453" },
                { "trs", "42afc5e7c464d91de4a73f5834432f7010783e2abaedbde6a0513551d47f95c5b756d6fc6e29c6a2ab7283445603c0cc16e517366d378ba969365b129d534928" },
                { "uk", "145ed636d8a14b2b639b83b4b5e801bf0109837b82633641017953222146fccbd165d9ab0e09b937d695276ef621db80c92ef6c81b217c6dabcd9780d40b2576" },
                { "ur", "d3594c937ecc549bf0523ba32c676c9c92c84dddb51a0ab1fc51b862587b1a8b143a47cded69616ef2cd33577f79e1467893493f70bcab5974ec6f0164459e9c" },
                { "uz", "41c1904775f75deab329e9ed9d2ef6494d44c7df3f8d9de2d1ebb8654b469d5f428d66d5b3046fa4a2c33134f6920cdb4e02a80f91e7624d16f3b01b661ec62f" },
                { "vi", "c6d356597b042831e261959e93f12503181d926fcf3b4448a0aeee9e6e6abbd513e3d3360c85cf0101bc51ac0c594c58b6a9f9d9f6dcba1bfd449f463d3ed583" },
                { "xh", "8a98272022f06c5738d03669f1849c763e235ee1983d1d4b9579c94a06464ed3aaf013660cda82ca74a4a7390690503dd0ca201284c918070a99d7965585a79e" },
                { "zh-CN", "49a63329395cf808501d0f9df53c626dd137a68bed31e7809a9185f49be91a1c1640a14d227cbe6a7c387ecabe9b57139b2983b32f3b470dbb4f318142c08e8b" },
                { "zh-TW", "d308ea4409838da6f6029831a31a9e973df5b6b8cdb0b187fb19f58ce9a3be32c69fae00ebe41561386831ff6de71041cb4649d6fb28a6cb42ae500e6e7031f2" }
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
            const string knownVersion = "124.0";
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
