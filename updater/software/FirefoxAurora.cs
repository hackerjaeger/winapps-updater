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
        private const string currentVersion = "122.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "023343b1b4dd858b618ffb1e12c7b5826b339e411f3864f15e1bcc299d744d7456ca285730f91535b54bc20845cc8a78d9024464fbd2f33b204c64c4b22503af" },
                { "af", "b7cf39c1e3d4966304dc419367e859e2c55c49dc7606586df50da4cbac9a194c698a552f73fabd3a9ba626cd4aa87bb37a155e3a2b84e160f55dedf04f9a645e" },
                { "an", "5b7827ed3333a9cd1ca5c80b6aaa5e9ef8fab38cd4d2942f6d80515246f8943309e5ba538b90cc498707987f2c890ebf9958e18fce703ea119f97a080cc22701" },
                { "ar", "f36999d580ae0130a9f88cc9989f280c49c56f6c6c46e9a52a1caabfd2aebdbc8f39a2dd02528c003eca233ef598a9ca5f58bd0f95dc592552fc419bf3bf66e0" },
                { "ast", "35623f69a123f94eca52784490e15b2b57f9d7f2852cb5d12cf876502523fc9b84ae4fa817a4d27b2e0867c097352755141b1f3156955816e642abfad1d2e092" },
                { "az", "802d7ccf93e0608197411fcf2cf01c8e085cd40cfd048a51175601e6fc901bde002932ddfe792ce41f3d3528009b06181096123917241881b9a177dc568fc237" },
                { "be", "974445beecab5ffa7fad7e447a7e325ed5abd5c02528971839a0c788d58adbd3cca5d64ee6a6f5b039b803438c90fbcee179a5b59170ef09d4dd7fc01c5987de" },
                { "bg", "9728774a8f9aadd346600395b86fed71924387316e0f3a5acd34a5babf910469bb1b4dc7e71667212f78afdd6eb1dd86761da1addfd03693c8726267cd461ef9" },
                { "bn", "87acfa3f8cfebb49a96572f4a82da929d1d808e83c2441b6ecaff510eb9a2769a8990b6b0408e047ed0534300861cc02db37cacd12aeed6ebe38b29c0d74c6bc" },
                { "br", "0b41672d79b16dc7026156f3569e9be6711c36b416e49214670a395c9421975bc1142c079cb8718867e9a5067641fdb49cb9ccf3a1381ccd5df7ef82490ac1db" },
                { "bs", "628787bb855af4e508ee97aa9565baa8de85d064f97393b3c058397689a8e64b2fc533c6b43368cf3b528fc2a5045b56c7b1242c940ee4c8401ed3a699a84063" },
                { "ca", "1c8b4594f61ab74ffb21350d90cb5099a626b8ae2b417cf42462f0090634bfe583f6d5eef973a87e54949e6898f7583c884049197955086cb03e1a541a62b298" },
                { "cak", "fc1721fb5c755f039fe47959c037f4aa748d872737cc9455fe7bbc972605185784c5a31146c6fd74d2554262043df3429e6df1f36751b0cbeff02c04c81c47a4" },
                { "cs", "2039d7645d2f6f6c5e5b4c534a6254d773d36b1d3e1ad6491729bbc0cb0b0bd220fe7aa78561330c43bedf2b86e5d537935b97f4b41fb7e8075705ea54a3c741" },
                { "cy", "6ce7b67bd85a0d0655099c61c83a3d84e7ceb7cd89264061cef0bfc4ab869cec983421cf4e0279c0af5edc0316314877efb012e8d53d6e989a7225450cdd924c" },
                { "da", "a55ba2c6b1a87b3f851f96764f13ca432f6c78c0a2f323e5072c355fd3d1c8decaf6aeb53a46eae65a191a64e6ff431c2fd037806236a3be317702209fa114c2" },
                { "de", "52e0ff52f16c382d4944cfde7aaaee41ea15b12108e3e29c53d967f52a8dc61125627b5f3cab77299b3977be85c6d64d919e518c19ef567bb235a24831837eb4" },
                { "dsb", "c769862377e0189aeaa5de9e56c2d61aba2c88f4a236c11fd3cd286657161e288919028f5cdbfd138498e7f894ec9abb6d9e903ef74f1a871f6b144ab05adb4f" },
                { "el", "9fcb436c734e28f85e89bbbb1e63932191c9c8be7eadb41abe8951062a232e8377e708a347f231d2beafa8d1598aee185c9eb91be6a25601a609c2d11149786a" },
                { "en-CA", "0e7b364f18e4384b90293cc5a3407c6d79b97704212a5b880163787d403bdedcc3c79c17d448c51f462382595830abc10925f3ad820de2dd9408f7c9ce10ae7e" },
                { "en-GB", "3ad0b57d16513d1a5a7fd7b9d121deeb379056eb8733f8e3ec32f90731ed2e0aa92cfb67a0dc9796bdc3d3ef3e7c68e8b3edd2d0b6f5830701f7cb0a0bec7083" },
                { "en-US", "1bce5cb2c86b56d3d07ac1032cd21efc4c351bb40c81fba1d9a77c7b755cfa35c602942e631020a15718b0938eca087c8d09c85da6ea83533ac770fc176561fc" },
                { "eo", "36edb0400064ad89562ea71c6152bbe76067236cef5e78c1ad016187bf7bdea87675a9cc692c76320ece461d76cb01fd4a7eea9a641f14627b8984bd13d4ecef" },
                { "es-AR", "4daefe825e5c809875dbabfe82b9d1050bed5f95998624bb878d7b662dcd8f2d75adcd9ef7a4b8356955e1a30ce267c433788148c685b193acfeb530f70b9e13" },
                { "es-CL", "e6eac2c4b4662d95a0bf443e5ba8af2570ff27c6bbd3b5648a4d57434e13a9fe4c513e123e9c3240b56dc2069b523c9c4bb88743d3b4ac4b8b8ad0fccdfc6c94" },
                { "es-ES", "1a47944a2e0145f343a0be2bfe29233d84a96ec26d7b421d5f8f032b84c612c386d43d2c30951e9b536852de2d6a7a7247b98116d7b9c14062b46f44a810c8bf" },
                { "es-MX", "fa078078c7379a27d21bbf1dc9df6394669e0375502f1f80128e7a10b49d1c2546d0bd5bedc63a3385fe229e227d6027b8eaf7d42eabae26046ca5241c8da25f" },
                { "et", "c6082e83859e389b9fd49eb5abf6c56da293e93a59fdcceb13723c81436b0c599a2453b86d24b3b9e2c316f1113c5f28cfe477201a716260c6914131957b7183" },
                { "eu", "6145af470021eed98168286dda79c7b338f0fb0dc923765e67ced2d9a6c57fedddf64d700a99182c400f82fb8e04b882bb98b829b1d36a8d70d1d71c247ce94f" },
                { "fa", "7cc551aaf349e5462660247a1ee61fe35751da6d2db19a844d27e851ac35b71dc22754cd72401be1dddd2e13634a6168de589f45bdcde301097b5f239fc6925f" },
                { "ff", "fd762b319940e00aa9f334a4396543f0507a6280a1e202713c7febf59658e569c3c63072f1c8c98644aa52098652714f7c2a10ff50d7f76a02563a590a7805db" },
                { "fi", "8abe4d3073ee5404606d7c815e52afeb5f113d49bf4b1ac12c1202bdd45a8a1eec2b6555610ca1b8da53c3d8c5ab5acd5924cd15c98d78c5c090a3fa8e8405b9" },
                { "fr", "2e0362b0c7564e12a6959b92f2045caff0451468c48530666a77e6367882dda22d24932a1e458f934ef9928b7e7ac63628f8a5242e37aedced1734e33f3b06d1" },
                { "fur", "ea9d06199358baf3a5dc78cee28dc9889de48cce92f8282eeea94465dd30ffb2aeb13dd8d1a508be8e3e332cf3490a213ad9b456a9ab634959d5abe3ab92fcde" },
                { "fy-NL", "a8ba7dec61dae62c1dc59297a76bb7a934cac75f2527eebe12e9231fc6ca06b503e9464d06e355628e44ec6eaaa4e7b084ec2e657ab349cf01e0afbb22a47a21" },
                { "ga-IE", "66a50e703389dff96551b4186dcbff7a0b2d3d2097d40d85324854850ba8edb8dce28ac919ac9de7d6a43cd1cbba618cb1abefedfc83fe6c2369f56790501a16" },
                { "gd", "1fd2a11d99d892ff7e37800e2875d3f95fb0bda45992a547761c0dd7ea4ab4bb157f18ebb660995eeca789c15f70316ec7c99a8c46caed27eda440b77776e99c" },
                { "gl", "8aae42e3aed34aa9e27f598f36ba4adb0da2797e276a0910f6c69cf1a8a43cb40e8a28782dd92e4cf0970e303ff2af2cd043382fe069431064e894d81a51fe73" },
                { "gn", "dffe3ae0561b9cc44a24c99752ad8a9239b8e3e61f5956e062de61560a9133fc53647057567d0b97f0a5bc90b655fa9b43ba581c6b98c7fc9d604029f36f79bb" },
                { "gu-IN", "f8e927328d6973f5c24bea1f309082d65c6d13e0aec9d184fdc2aa476f8a8f50db3e45e93cd9dd28468b9c9f9ab5f8187fc0c4e60796d06aaa1737c5160610f8" },
                { "he", "a604b4887319940802855fee763a5bba806d82c264ec7888365120bcf1884babb766e29cfb8d222acb4908a514c74956a670ddde2cc5133ad571ab09a9b0f84f" },
                { "hi-IN", "514cea135f7ed96a94f30e89e05ca60a3c415b61ad84bc6c06c224bef2f8891e00813b084ebce2f6354aae415050e2ad7198c99747c8f73dece706afc365e67f" },
                { "hr", "9510ea66da3a890f8872eec6ff9894dfe741ccea7233a11228e2381e5f75b2d28fd18242e101d1c06e8ffa83781282fc120c6c855693c7dc4b455d6802fb2e66" },
                { "hsb", "2ebbd7d3b752c8aedd501f7c47edf2c0b961d9660a8cad9521fe0e81ef71f993136c4a3f1ea92997327bd685d3e77d956e00b28ae85ed4170f90e2b38b87ad9c" },
                { "hu", "6b92b188ccb3a47a74d06edf765148f1faf295461fb8fd1a95045833efd266d7f876d5344b232f15f13e95d8b7736c844c678f2a94e5ab2eb230cda7e5738122" },
                { "hy-AM", "51c83778e78da466b89a3b1a17ed3a926dca0d0e94deaa7324b2484721fcb82e5af6b8f3377269e0b0999098b8b20400a3dc5e07799ae66b6198638820b03231" },
                { "ia", "d82bf7f221175917d9715931d239213b78b4cde5732df06c0b8cc340676bd8afc72b1586eda6595687fe97c45af19980c81fb0726fc6fa5afc590107fae84d21" },
                { "id", "d0aa3a5d8f6e9e7b7f4f38875bd0ed41101a86b05b4d453a6f94f50e00e6aef5e3ecbe9181e698447393075116491b516343f27f6a37f7d2775cac788769e49a" },
                { "is", "8c2e815e44fabfd87af2f64b67d863e96a548a8be9507a3a3ccdeb654af3e0fdaa99a76e9d07dc645620e81f2402088111788a83857918db0b49f5cbeb86a7e8" },
                { "it", "5a8d1ec142595b79a2338880ad12ac90e374c648d53aafb1b100db0ba640e90e2f8f0e37810d85df8df7c239a27f915b8744b387de0d7956ebab8869b260370d" },
                { "ja", "02a788fe4628ab1570c188ba616845eb612ffa903bf1c1eedcef1f49d9e89184bc08609091bf2cd7de71d8fb4be5207d792bf17656156f29093c92496e42b43d" },
                { "ka", "4d10625c966f3297b3bde8b21ec16dfd05819d184c0de4985b2bc956a244d417e8ac7d3ae8e61902a0a56c8347c1f1b02131a64196ecd913b3438160135a049e" },
                { "kab", "4806d198cd39110e4a820e5998bf1b247d728c5a06219a8c575e7ff98a75f8d4097543bf520d2907239150d8f4234fdf63ada2b13eb48a4258a9f7d312eb3f98" },
                { "kk", "4a6051e9bda1cb915d40c0a95f89ae72417dd81835598f78f1436ff00dd5c55490a878a7743fc6217e98cc5da25384f10f4f6ab2637fe9ec3400fd6771149ee1" },
                { "km", "8cdcf82bfd2858e915486df7a904e88ffcdcda001f0a2f926555adb355de4823197b16cc5f178fc68e5fef55642992258ec2e964a125ca09cfa75867c0821840" },
                { "kn", "ee4ef0e3f43e98e9279223e7a1da9d850d23ac74ff87d8603c98d6e1c9ab7447767ec1132743c6448dbf4422f8e3aaf0ecf451436e6b83a1b2ad1074a85ae17b" },
                { "ko", "fa0eebe7d66b3e4da14010109568079b290c547f9bcd53a5a04aa284a6ac1b81a257f73a5d75b71ede6f0ff2bab714c81c1a86954f1180f36af664cbadb7b85a" },
                { "lij", "7f08ddedb956c857af6e5eadf88522894a9c4a5c9e982e1a848dba30120eac88f1a2dc26380ab33a71f1909053fdfcba905c6854d48b51f0b9b91c541eb84c17" },
                { "lt", "5096b322b41b7701aba0d632f95db59f84ea1e6c5d3fcc3ddf86d1078f423906028dfc57e3d557d50d6b5603f648df78b6ff9cefedfc68c4bd33496464e29cff" },
                { "lv", "26336cf34fb3481f283fcc3cc6a016be6b2b862b258a316129336a5d109f23dc8c9d6fc4fb40ae45abb434dfdc7bb9c1d4017e0dcf0b45a3bbed823d24eda905" },
                { "mk", "1dffc625411c402ebe9c9cc6779fb6be248dcaa1ff75e7d0fc169079fdc2045212d9654b6776d3fafa90227d1c4cf9f7bb1c99a4ae4f866b1c950ad9c721436d" },
                { "mr", "c9c1acf375f4ff76fa300bc8728f6cbdeb4dea77b65b1f8a70185f7d28da96a5131b9163534356f5ff0c29a04a0575e20f0f68e79401779d68a64623e838162d" },
                { "ms", "585494b8daa7ae672b21321e382d3c7e1499673c855dcb2e072151aef9edd058694d59850c6d4330893a00d96031a9d53f121c847716bb437fb573aba06d4c89" },
                { "my", "4a3c8fce131be07381598fc2c177c48d3673e71c66715ecfc0c5a49e7293527edf5bab446fd98affb7c6b3f4bd7906f786e49c06d286afaa7efab5dd34a51e73" },
                { "nb-NO", "ca4350a146ac315d5cfa84692ccd4f507fa9e75a6c27cc81a08b03150f3663d9ee14ad285a3e06bd458c1d1c043e0c325f80f18042bf1a593175287c8faf7d13" },
                { "ne-NP", "b9b9d8bf8f9ea5e6361ba473cb794a761a85b6599041a90756a87d77bef5cc63a4bdc31ef402fc55fe7110f0779132e8b088a315a0d9e02fcb6f4342ee0cc67e" },
                { "nl", "ed38ebda9a3df743ba1392d49f4d5fff1cdd006e4bd4137c071b937d2f57119b54935f9242d444393bb3c0a61b909409ff63d90e8e487fe3192d4e3d74b14f79" },
                { "nn-NO", "93fa5fe082d9dca6e7aad038bcb3c050b2e5a58ae54d85fec7e14ee81673df30c56dca496ac30cb3196ac7807007fde2f56679e74d0d795b827937098e02e672" },
                { "oc", "64d799c2d5f82e9548da02e8d1c72fcbbf407ad8f7eda696109bc389cefc9e3d8bb756ad57317ee41260663eb5867230d24b9b6b610a0591e5b7a4bbe39b74b9" },
                { "pa-IN", "adf6a1ac64c417a428d9aecba3ab44cfb39dfc1f08137fabb1464011ec415f17bb40a5a9494c2a5b36f282e4a54fbb69b35c26713f09f38ac611cf1e97e88e0d" },
                { "pl", "a193d43db6097037fef9b5ea5b7bec0fc245e0289845ef020d1a3e6e0e37d87a6be37d85da72c2f738a4833c2504a842791e2600c32199d8f1bccc4bb561bc46" },
                { "pt-BR", "5129320cd20e610d6f6013910aca085fe27bce965b7ed14f7f4c42c50f88f2bd60fd7ff64a63f1c1e1ad5b6335e2d53ba48b47a53f82819272d1b5ace9f627ed" },
                { "pt-PT", "b879582406a2be7c9552463739f4785eeff5690c159f9b7716d2aae57fc7e9c2dab88b442116127369ace539c88d51eb926168da3647628076e429d11c3ebb7d" },
                { "rm", "41ffcbfcd1bede47244a50929fee06147f76f70a97ef4d84fb590c0441090f2d294caba2709c734dc2cba9b43e57fa7b966debb9071f9b5bab04b904779eb946" },
                { "ro", "107f361e59661ec742714543b8012abe40088cc12fcdf727fc747cecb2c67e552b70d06887d0385427459e39de3289d300d715393ddef00112c336e686eef0b1" },
                { "ru", "c8b8005d8dca110416bc32386edddd36647cd170d75d28b4cd7a285380313246487b857529747d9c53a65e6fedd658544e559ec92966691a95270d3e4a35a1ad" },
                { "sat", "b23143099f9e98be338349403c2aba1ad2c3480da5c3b6ebf02b992d7def6c3a867e467bb626545b01d12162b1f98a165a77dea028e32b1893a112b47ef66050" },
                { "sc", "35ea6d7bd6b09a0675251490f2474704424915891315ac812d1167b36246b007203f8fdce5d4ca77a3388c3e63f8ad8f470efd18999de751e0f579da9f4cd921" },
                { "sco", "67459775584d9c65173ba36a09e2f44cd3b20c95455ee5a6b7ce5ba594b1090d476db2acbf4f5a21cc7f06dbdaca0bde6d597ac53e0dca4dfe9c4ff3374d909b" },
                { "si", "96939d0c8355bf9d04264a40546ab5cbb1b3c1e6f322c3028eabc44d14449124eee522cbdb4c23e27616353fc498a5b6595b0507a167cc6a286da5a6063fea64" },
                { "sk", "5718e6d8a6917221c5c539de6bf7fc17394a60d3d41d91e4e407e2fae4116e79d602f08ae61153ece5501351e9a9abb432e18d0782b6b3ca0080d47c74d937f9" },
                { "sl", "e6be35a104dfc1c227336d50586456d18e777c072a59b4c898c927dd51c48f7aaeb3312db71386cd1dd03cec44c0ea0537642fae6b2c165349bfc941eb9b3646" },
                { "son", "d0ad0c7ac85d7339a9cfd6889a0a10938ff3ab4800a7785b5957aea6390c810fdb864bceb4ba5bb605ab292b66985a166e1ca2a821e39c4b321dae96469f53c8" },
                { "sq", "656b1a7b065fb5856c569af8f162dd408c4cbf29f9073db7e31e046a03a68d48121862f9a7ff679399750839dfc723771dabd2a9be4a7b5e41d5a650643ad5f3" },
                { "sr", "e0fc935692b29d6d29e2d54ab7865874dc582de0f6d456cf037594b6b868af3b97aa6308b09d54062ded02d5b3f16943a4cc4d3792f4b976ffff07226cefcfc5" },
                { "sv-SE", "fa9bde3eb06c97c12ac93f68c45bf9ce5c3f6a503e8acc122ba84aec4306106e067fa51a277dbc658e1dd09bcec1ebc1fa4fd5ace5787a6e89b13f1b9b1f61e9" },
                { "szl", "cec321e410397c321cd0b52be6d0d0632aadace347cfa782cc85542623fc20df6da2f5611cb9bf059216250b7db03ce327cac0ef85cf795e119f923c93d1492f" },
                { "ta", "a2317ceee749511ad7659dd12da4641a2ba5cd94afa3aac11917e11cc4e5199b49760b962391261734114c56e855dfed6d6896db12abc1818e4f3e3d8033745b" },
                { "te", "f58e4a4062452eee3bee61e274a538445d61d8c4389b6a9bc08b37116af8548259057b5b5e8b27b57de73de5f5dd31af373221acc09f1057202c33173e6f3e94" },
                { "tg", "eda46088811145286738f31b60d3bacad2939941b28fa1d2280fd4360e0b0fc33aebcde27ca218936375a351866c506914ac178a5af7db91a05bd40f72cb93c6" },
                { "th", "f2760e9bacb9086aa975ded347059cce55c0b95bbc133d3361f690721c04203e7a2d0296e8aaf72193fb6e05272c0e41e2f18aa294ad0367fac9712027c6d6d4" },
                { "tl", "790e48f9832b2661b368c9adc82e36d4213cf87b55f4a3c18bc9899c61ebb9b8fade85e17de64f3c6d80b459ac20a7f7013ced23377f4205e44fc0729da2d170" },
                { "tr", "a76af3efbc888a6c01867dd1e6e326e0f1e3e7e37ff82e1464030e0142100f904294375add61c8bb6e0fe8849f0d22b97125ead07f2bdc62b31110ba7355d2ac" },
                { "trs", "19fb59ff8eb211bcc3b955ec1600c55d8a2566411bf7ddce50fd017b43f4152333c2c8c04e2fd52e97a3d449afd074c7bdc363d731e71e2247c96de69c09e507" },
                { "uk", "ac129c1bec8f651bdff61040bdc982b5dddc9b94922f4d34ac4a5f6aa086640980e7b52ded7d4d695d815238e64f49db5b7c4fc1f944c27d2e60839aacd14bbc" },
                { "ur", "9769c6ecd53a70b7b1373761eaf83d651eea56594a445dffec5dd3e63c124a048a3f41b57b0ad7b62a574210b9813d120a333198115200c0963eb17e5b1a94c2" },
                { "uz", "9c0890ab5d1ab5ee7b6a016da0d1ec862ba3bdf55741b3b0b46e6710466f340036522f29be98fa05a3239753de69666658b8602762cfea654bf8aa4e924d9291" },
                { "vi", "d318abf52899fc0cdcc432bc06930fad9314c762748c576177d56c5d062facafc9db67c64a7667eb226f20f55e5a3fd28d8fadaeb9c9a31e8ed53ba1c865a5a0" },
                { "xh", "c416cce27f6c3221d7ed06e9a8e46120e48a6774b93fa5ae5e8dc7c5cce6be373b6c46634805ffee72c110c8e81ff7498ed9082cce105ec6b4c1d799ae66364a" },
                { "zh-CN", "5500a1e9945fb950b97a7b2978eb722ea6b2a30894a76007e205b82629d7b7e071e3f1a01f1bb23fda27195f5c895784f4ae71e181060512237109c5679a537f" },
                { "zh-TW", "c1e414d05d9f22be206b6d487dd480620d78a974daf8576d3df45f6c4a1970eca33efc2dbcf0f2f5e71a8336a24d0caac85ccd3965e814c4347730e0c1f7f17e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "c562e62c9627a5ad4c429c4249746ec82eadb4ab81bdaa80d431f5bd8abafcbe540c80841fbde64f723c49671747923643a9690595db7a3ba2f85bc1b0e42330" },
                { "af", "638fa803dd87bef2754843cb160bb37348d3e6704add09de43d705b219e20b9c198780b4ee63b26e579b80357150fdb73e593b436e26273fe607e64e8a80dff8" },
                { "an", "c124905729c2345961ead9123ede43f558de89f35d28045dcb8711bb7f025e8928fdd77cefb04f93f4329abd40ace98da00317258e23c83588ef2e5f393ecbd4" },
                { "ar", "597b348960c3db8cdc6a2dd972cf2dd47e748640d9ba0d45f4f4217378f7c8216fd5a2f52acd268cab008406529faab80ef262257f817968c5bbe07ca8a32251" },
                { "ast", "9ef032176255231fb6a361228e1cfbd9bcc8f288e011f2fb8d54e914a1283181bb68a81adfc1a91b825aa92fe776c2f2a72fbccf7e2343aef37f477ff280986a" },
                { "az", "1383b8b2cf6929999ed26d7191662aa973a26afda3ab2a04c33b396a57ac653a8bacbedc15776cde6064772dd16f87b337c75becbc4663cc19cce20b503dde8a" },
                { "be", "d54f1383952ec4cfd8fd44ecaa402304f416780f240208d6983756d865e54af64e40f47674b813e5db4a9617d0f426cda7eac009f898df51894efc9a9d6872ea" },
                { "bg", "53db99438ea9ed0e8149f1aa8e3154c50c1cd2ddb88cda717167f39d0434a2ff6f5a6aa848c2bb30cce27981c48577e99fdf9b234c0cf84346c619c0779161b2" },
                { "bn", "6b7d1f225f309ff3f7a9a3be402161080825ee9dff935eb14cabb7c2317b86a059943b3ec349ec5e7a71a4ef2da5852a106d312937066c7add6ecc99eeb81a32" },
                { "br", "261c58ca8a004ccca9c1109a10116ef4b165b4f080f2caf6daa9b178c2ff578150aece0a7248de82673be0be3eb442bf1d4317e650499ee369094164fbd7d2de" },
                { "bs", "f8b19f3745e112d5ece918beccd7cc3a7163809e0fcf6666b23575a268682abd4192643e655a6adc8e4edbb181b03bb35408e89d69308f76ecc048f46471b4d8" },
                { "ca", "b80d7dea2e007216bbe67018c6453bc8c7a25a45650a990a690b321f73ec1941c60aa35bfd64173343d14322d3a729e90f5ca0db93ecfa72cd67901ed881b44a" },
                { "cak", "fa0a2d1a91150e01d0d3bdaa0178b4aa017c75ab9b554b559829cca57b5684922bc2fda0cb289c404358462fa52a4e342aded5a711b08c60e014974e9c7f674a" },
                { "cs", "e8a874ea2d972fbbd1831cc80fb7cae48013d60a8a82b2501f3b6b1282f32f6276ae8023e293d958a96c5cbb7ff2db55a650b8087b2d48dbaf49f317b6dd6905" },
                { "cy", "696696856bec09759a5d2006531650ed2fd9078984025a844a54c25a2dc25d9730978ea9eb5a4ad53c9a2535b959917cebf831568532cc58aab38d09efa2cab8" },
                { "da", "f382d91972fed8fa13e06b1068951c2ab5b5d064618443535344e3493dad1735d2788d54a4f86e359d6d8d7489e16ec1c02faa5930dd4b1167f4cba908fe7c47" },
                { "de", "f1dc6ab99469fc72bebda0a57e7188d0a2a13fe8628e604b640c765436ea9101de6484be3bb9e5a3cac5ddb69edfd52cd350c746c16d43534806d1bb2735d400" },
                { "dsb", "c634e2b3be1f5bc785a8fa1ddeff1f74c8a7c446746bce923ed3d5240b91811c4299771bb3d5f10725e380d689546c3852afa826df83757a16cc3603f8a4e3b4" },
                { "el", "5ee09d8de8d0e72ee1402dd657068b5a42f203a886c715a3f89c2b5b3c6ed0f68396f33b958584170b7958512846935bdb9ac7267a8872ebccbd7cc5afe338d6" },
                { "en-CA", "39426beab744a57a8daa1cbebb86bfbb5c04e715593d363c142a1575f8d5bcf38058876825c27db78289bf993e3e547bc19826a408227b9547ce4645c1dda2be" },
                { "en-GB", "c80998524ccc44d903abd3ca25445954540740d45be59967260e44ce59166492fb79820a282000be0b6110fbb0caceaf9d9e0a9077a48091d3dcdb9b05b47e2c" },
                { "en-US", "72bbe37f037e072fee2505304686c8cddb909c04dd51835d2213dd7bda2e3be55def5ddde80621372980dc3f8178d4e31d8c2bb5fff640c0d419f87a6a263f36" },
                { "eo", "5c8ec0d66150d4496eed3ccf6bf826675011e1050a2cb49e16ea972bd3f804aadd715055b8678f6d73214e0be1a01173155b732c17b962cce0da587420ee4847" },
                { "es-AR", "0242456ff967f7ff5656827468277e76c2f2f2a075aaf57443be97fc28d548d87f5f481528a083d6dc1c98ace32c2009237e85b85426acc85cb9a8aa9353abf4" },
                { "es-CL", "a388a9266e1439805ef034a55ce2b9365c85c24023b86cf4ab68fd9427cec99e45d6847648495c2280914b087d95cc646152c9e9522c18c04f1421e8fecf4cbc" },
                { "es-ES", "1adaf6a18a1a4c1de32ad9646b34bc9f66b78cba4886189c49566cff50ea8c16ae295a5250f977ec135109aa6a8f05bb80ccde8818865eeb7658745232125fcd" },
                { "es-MX", "c1348a03c493f9e7150689d011bfb296bb980262f48c3745dddf3434998f52d0c6b4f8657bc525438148fb2546970d69236d754c5dc4487c8093b3d4b302b165" },
                { "et", "4ff91d8b7af9c54020c15c68c894d733579972ea382f22c4c8d5525e601ac86f07e693e834a54abe4a001ff07423bb190bd09101adcbf356d9cba9a07110caee" },
                { "eu", "790a0089d737ce68df435702e37e09d21b8c8e18c18f39ffeeb3d2c3c2e247233043e8ca00d93c8060e11416f14d4ecedfeafdc88d10103a422ac82169b9e0b4" },
                { "fa", "4eff7e79e215fbd79a596ad185d2bc20a44f941e00acec7a6d74112ede40c0525c7ab5e28e9fed2e3c490e13e4fb48ce9f3766d556b9295226264ad42182793b" },
                { "ff", "3e88f4d3f3d05bf15ec6791285ccaa05818a58abe08965f35e33611129ba404c2142cf5e29ffb1c099cdbbe7099107d71c8a2f37825325e756b2f544c82bd9c3" },
                { "fi", "26498bfa9634b02e7a31107a7faa694ef1a6c43738544578a4b3fdfb1be9dcdf5eca01a93ec0fb45db9fa3d972039eacc4b5ade4722d3bca43cb972b2b89e5e4" },
                { "fr", "0c042ec008943307f05f3f56e2aaa945ab79e9e0866d39a787a5ee731bbf4c2c5d0fe1e88d4e1c831d9006727d205ea48db8dda701414dedb063380d65bb2cbf" },
                { "fur", "5b1d1239429300ebec3c86ee92e066e1b044428c18aaae16ced3e893d2742831b35e166cf44b637bcc67782132cc6f3e115cc62a77363bfa43e190bec378f18e" },
                { "fy-NL", "825614d36284ab8ba15c5d059e6e25bd4cb86fcd7e6ef8ccf994baa8d5ee78eae63301a97160a01969f4681c63ff6ec55f642e3238ebd363142ce9f2959c7eab" },
                { "ga-IE", "d52905e6a69cb11e3a48179de3dea44f510f8db699730587d20f907c2ee49eba07054ad3afe1ebc09c4ca5808638c7ecace72cff175fd61f74c922b333e08e26" },
                { "gd", "be812209f69dbc34159b21b610d8a8bad385802b44fb6896b6d089373da6050dda0c5bb1f527bb2df452e6d9e34d4cd111105a7d1b2d2d21b509d1f8ad887533" },
                { "gl", "7dd2fb85ac2cf7fbc5131f157ce49f50139fb1d956f7eb5cb65a3fad61386f0dbac48739080cd173ba91039a999719dd9d0aee4b274f7934750c093d57d2e6c7" },
                { "gn", "caca9568c0380524a1f44ec7c5cb6e3b0015ae89956a6dab85aa00232d5425f2dd410c9bab007a83bbfb27eba79e104672f36a876da2986b767a4f4f1b0e27ce" },
                { "gu-IN", "77590ce19b8ce94c1d8383a013e38b1044f304373ab14d5dd0d0c066f0dcf692199a1387d0a24671d0e1e5b3e80d11ced6a23c816cafc89e1bf208374496e1c8" },
                { "he", "fdf7201936a63526571dff1deec73fdb709ad556ae408f41a2702820b0da1b84c886d923503952c95cb1bb51646e8206e2d262f14c2aba8bf2e66a9e8a50689b" },
                { "hi-IN", "9cd050266c5682e57462162174d9add85125d5f372a39964fdca1d4cef00332057d2b096cfa247405432bad7ba968b6364b2c4cdc40aef944a904222f3473f9d" },
                { "hr", "f8d9313f581dc4748d7132355254c8dcda307b1e331bd187fbf0d6586990aa16a129cbbc312a1c33cb0561f0149ce76778c33bd940cdfd31b1a6419cbc9f20b9" },
                { "hsb", "3d9e9a63923177c03a81543b4b6b91728f1f8cb3eb0e98bfd2741c0aed56aa584fc72ba1b51448b5506cf44b38fc7f5edd596c849c2c35f336f86cf27f02a815" },
                { "hu", "d9255314d2c328c8fd8f435af8b2fac06739811793a5da863d4442ad274bcc8d95af2cf0f14639e6413bd703fd1ec805d2f9eee9d874f3e540da9e34a9535373" },
                { "hy-AM", "c0e04bbe548b0ae5647bf0060b93a8dd7c4458b6ed8e129c4fcf70337e21b237d3c801403dab18cf3a996b630d3eac7f73b1e74e84467be36376c1196d9a7450" },
                { "ia", "bc6cfceb9d0203094471f06794bb7753d586bde207e780494af2abed7e2dc60215dcf48b79600ab94672796675b4152b7ff9d12a6c5d7795b0f9359a662a8c3b" },
                { "id", "2791d22efa9f3e4bc6064f61b823f025a82b98ec6d798586536aa60424cd9c1f528e8aa39f3b4562651a99051349577de0c37191535f34f60d3c8437478bd9c0" },
                { "is", "71285a471c9356228927e4c8bb9e9ed1c7631af15bacae38d5536d3f23fbc8c6cd6187fc07aee51f744b5a37345445fabdbd79335b4c887bb00f797b94a08ac7" },
                { "it", "e611fc2421834f8cfcaa06a745ccb69d7378c3c54dc5d2f1d4de04a0fe44a12aa92c05fbee01f544dfa485efe992f57931773ac6187da29d3ef16ee3162cd088" },
                { "ja", "c0c943882c5c8f69ca5ad385919b76ded354c016c1f959eeddee2eafefe8457ead3042cc0e3bedce358fbb7c79cdcf7d3cfde0e376937695f9e735a59a90d82c" },
                { "ka", "b33dfed18f8f0b2d80860eea8b67530c5c3a7febc6416b2da21a6e3894dcea1f9776874584ef7715e9939ee7c40d6a0a4ddf74bab5997fa0f169851e68fae7e5" },
                { "kab", "da04f007b8f934c7689bbb2eb789486735415cb344136f414e4c0ecc3a55f0a3e1efaa8de00fb0d0ecc79d9eb803aeab11a6a6da85bb004cb4343d8a13ad2866" },
                { "kk", "22ae9655abb1ee27af1aaa5dd06fea08e455b06cabd271c1b657089149932c6feb3eacc56eb65f2e7b62658de883f186ca77762fba53f21b8de9405f89f0d85a" },
                { "km", "1be7c999bc6c2b8e244848582346ec4d7f0adaca72c938e875ed4b05861053bd54cc02535360cfc97d8adc95dda143e503dd008015e0427124e2443ac778ea96" },
                { "kn", "2528c5815abbe36e6878c03ab0f874799a8823e10e6b39a7b4ba210c7bb7a9c5d42c27d8d76532ef8f54b318e9f7337a0710f24d2b934d0c453094215c3d4a4c" },
                { "ko", "73121228a70d941f8a8a9117d13a022da73884d80be19e89325df350332c47a291527b90420a4c43871b53a148222d69f9a13c7bc6e950f3f76615833068dfef" },
                { "lij", "491eef327290ca43e8078457af02ac7a8d663322320ec650f3eabe058e39dca821c5cb219e7629190bd2a218939a447f56784bc0385575d4c313b1e20d1da792" },
                { "lt", "0e0ee208aa59563f843a7b9495ae808c13da68b3c4367d26b886a6461de8505f51ab62713e8667b964726242294cb5faf4ada3c5ff05d0be280d9f670e9451b1" },
                { "lv", "aab667dc69a903b12f6f5e124702f38c6057812ae64446b423faae987ab67082961a8941e65e66fd2744e1541994fb8782dbac091a58c45c1d0ce9e07ea74c2b" },
                { "mk", "aa150a0a2b7cb105a557c93c709b006880bee31f3a32c7df51d187d233a0becd4fa80157964ce53a34b44499b7f5f9db3fa96ef5c3f9bc01a9b6b23e202380f3" },
                { "mr", "cd3c42eba6fcbf8642a60e0362cb26ddf9a6650e892d3fe70362089f2135138ff6f5e58b3ccc3773623022cb6bc1bf7a9c4cbae83c79cdddd1ef3e5db5813c49" },
                { "ms", "75d8f623c54dc56e70eaa906a0bd8ac1ae300aeaa351cdd23fc3b734375b2dde3f23b543a80755d1bf18524dab9bd6536b0c9141db4782bb74c07b5cbf3876d2" },
                { "my", "754a0066cff4347c8ea24377ff804413059dc632971c5bd4aa6e998027d22f90bd43c0e9a171a2edd7906dd4e0732b442f1bea0f5410c5885a776e1af9055174" },
                { "nb-NO", "41ccdda8f3e3ab00345935219ced4ead1300a8e4e6e33430ded3a1aae727361c1012a79d8ee3fd5cf7134726d3f390d45ca7b345af4d55ada6e2424828333aaf" },
                { "ne-NP", "f8293f063d6ad077054aec71424fa6901fa12a98bc4db67c0da3555dd8eaa9e54e3b4c47874c7edb7df0f042708fbeeb81225b2d1c2631299389051c5b668af6" },
                { "nl", "62034eb33d72be8ab7e0eb7e5df808e13362b7836d6e8a67336751f4fb325994b29f4f2a1955f1c649bc9933b5403eb35b0e5b50dcd5a29f10941bd6029d22cf" },
                { "nn-NO", "d6e74c45b251d1cb5e911268ebc624b4f570411dee0473fcaaf10e9325b06c1e6d834aff5b67b41c6d16cde4c261502c0b90a9dc46197e564a1924b050d6ab2c" },
                { "oc", "a2a15980d38af5f6e72ffbfadb7494b0760e6746b57cb93292df25dbda048c6da2d428ba938898a818f7baddf2aa1c611c0f134bf635dcddb7aff6768b0ef69c" },
                { "pa-IN", "d043c788ac8bc3bc7df2738a38af96d2dd34dc4c8997266dbd2d3a65978ef3e39d7d4e96329da3a4a3ffbc468efba0578f8d3d9a1a7f0e923a5ae45a5bcbb7b9" },
                { "pl", "0c164ba3a1ff4c8eb5d30759b0a6b80124c8de39b5506df8b18768df9f8bb0c079123d05e803bc3eac1628bb366a20066ec82248ac1986fd9bf3369df486e7bc" },
                { "pt-BR", "7e77598e24e70801044317cedb43b0f55e878769107bad8b12d9f9fcde836f2190198d129d1339b1d595f8dfa420e5f0bd17566ec8a51cdccc537fddc58ea246" },
                { "pt-PT", "0fa6fdd20ffe218045c4c299408cb92b7a13631cbe8fd30bfc2f988e54312c6b4d40e2a651cc4a50eb048706316d8a92ce2f35a9254226ea82ffeaad98fc2271" },
                { "rm", "faa1f9ce035bdb080f77c6e5bb0a9146b4a7dfe5574536bfb4659e2e207671b7a1a7880b769e91843db96ecee4983e53f6a6041a0b87c8f3cc39254bd3a4dbb1" },
                { "ro", "21ee0a229616dfaf73cd4c45f9e9c360e46ae04f8863236b45a353a118bfcdfddd3504b805c74004831b0db1418c764d9feb27930d2db603993e12f4249996e7" },
                { "ru", "ce2a42541864c1af8e1ce562785a23509052d03e9bada8c9b9d84dc3be5a9a3752ee760e7e280f42a38ee19c3134ca39b9d6a968972db3f08f75a577033886c2" },
                { "sat", "c9a814f2510e19ce324796586b6603bf559023178fed3a56bd6c294910d34646e75a7eb68a37cd20edf7c2c73ced98e51c02ea38d1b6519d2d729861a077d1b5" },
                { "sc", "0f0cfe49c7e2e9891c5dd6d418469157dc2b5fcc19a07ba5c57ece126806e3caf7da52a9dee01ab32da71a326b88779c3ec583909ef797823d1931581ac53aed" },
                { "sco", "baaaa7cb4e8c1af52dcf935ff330242d415af036e358c546b659db836de543e2befd6421bd6eaec601d8385e7d0912ed29e07a6faba0e20a82bbe2748748f794" },
                { "si", "34f7126f2b38c85b528413a48ef6501100a828285c19717fefbc91c9574bb462fbdfeb5efea34604b9efa0ffd4f4bc15b8b7cbb1fe6511798514c7ded69017fe" },
                { "sk", "fcbd69b0c90b6e7e65ea5e59b02fce4541a07b2629f0aaf7ca2b9983c054a208f9daa23d314e599fa9c0fbd61c39b0068d23bc8ad8bb76cbc4bf80f989276d96" },
                { "sl", "1d8baee35589825c042a24dc17da1dbf6658e8dbeeb0b907f779d63ec8573685afe8d815ab1ec44ce08433104c2b5d8feff01f3bf857e9ddc81648630b5744fe" },
                { "son", "13eb7654336c8376b0dc752f1284a81a2920e70c18e117893ed628fabcf69f37132e7cc7a94d6ca9a3c34f7537da0dadf232041091eaeb3a17a619a8c4d2ba09" },
                { "sq", "6bdd3ca29746b79ed0cd82f010a5d57ec6d6b963e0e69e4086c30cb8ce2a896f35a374c1c6e43f0a96c5b0e75d3a1d5e77bcdb7e9562d9905a716634b02571be" },
                { "sr", "f3241457d9834cc7ee03caacf58979f591e043e545a02074c133db8c9bc8dbc33962ab88d88aca558a284da22f0db1ca049cc570a667c4dc55755f8a8355cdf9" },
                { "sv-SE", "f88a8e14430f3af95ae520a7e186eac483d059e203372d0d7e3f79ecd635cd14a057510677788ab630e90017dbfa5d75efe701d37587b0b3a765721081931967" },
                { "szl", "a33412ae07734918954195e61007fd20f7911dfc69d4debc52cc96eff634d9b5b0bc0d8c39e7e3476ab0709a5ae7c325e0b3f9b6b8fdccaea32d1873d4dcc930" },
                { "ta", "5ace4765721f9ed59321e1682b16fc6c830ef39e23d405714013acef56edf589c3bb413628cef4854277ee1505a362ca48176c8dfaf2e3ee7c537dc1812dd3c3" },
                { "te", "a6837bfca31d578ad2cbf15fbd5ea257f663e04eba11e36b1772ad05204b5506e238689cd7ff60f335091ad31833edf8900ff8bb857850a2ecdf86bf5fd7a4d3" },
                { "tg", "73b9c6e58031b874c06993c143ba52344a8ce05825cf62b6807cbbf20e1e5d29bc27f9a7c42ca7a3e6b619ca2e487113f3dc851b92a05ec07fc270e092151cfb" },
                { "th", "3117b7870ad78ef82661fecc46900bd5dca27350743a93884bfd729df3777cfaeb56758045bdb806ddbf3b257f2b58e6ef0aa7c8d1fedbb15f0035fb96b7028b" },
                { "tl", "4b580979936dda42642e2942536d3787c6dec36d1d24ec34526f698a09f708551268e22798e361db3f10e3be4430308e068af626f30752d58a563dba240eeb9b" },
                { "tr", "eaae727835190a6d5eccfbbd474ebf2c1011d818f22fa723840ba39e00c26e9608aba832b9a6c7e6bde121f16fe5711f2e6eace8eac0acd70fa471aa733980a2" },
                { "trs", "7a52a1c327e71d2a75f3b1e636eb7796bd6c3906397e93e20083d52b37510eaede5229a3d9fa82f94dc13644ffc10457c1a9bafcae0b79385b2980b16d2429ce" },
                { "uk", "e0a6ef40168685b0949291d28a5c61cef187baa14cd3bc747ff99c782d355cc3d7d2dcdeb8a01f9d3f7763fd4fe0a72addc9701124ff21bf2c4dada27a57e222" },
                { "ur", "f19ad477b0bb710079457a5d1ca93f07f82e1cd6c84668d3300487f6758918b12074f3c4aaf4b550e8518ef1588700f37aa66d98ff3bf83f7fc2fc5db7416ae3" },
                { "uz", "e1c241be20ddda6860a357c0681046598d6969a6b7e5882d8bea65234a37590ac03eb7800af5ff90c7780997f8d6e69f5677777835069765aff0d42fd8ae0a4a" },
                { "vi", "0a6715cc178b98a7b43b312f53ec862dc499ca10764b9f989e1b1beff80d4a83ddb88e4c23591da22f114d2cd5ddf02bfc7fe0b603519f9e1a40c6064b2ceed9" },
                { "xh", "3aecce678be3bede63a1b6e8995ab5a1b331a2ca1b76d7ea86d2868730d36c336cca75f5ff61e2c853074555e635a617908e551c23d7859c2193e9374cf962f1" },
                { "zh-CN", "7b311f69caed7a65cbc0e33d89d21bc4fc6123c98a6b0e8c093e3a8ae96ec7aeb493aa3863161c94063e3fee68240b8b60e1cac6b84c436b1c6be241cfbe5480" },
                { "zh-TW", "5a265607d3b90d1a274873a06629a05b49d533c4b6cfad98c1d8b7f3d3970afff49dc86a771edf0671085e0afc42a572d22148b37c68de38329448b64c67084d" }
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
